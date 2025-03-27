using AutoMapper;
using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;

namespace EduTrailblaze.Services
{
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<CourseInstructor, int> _courseInstructorRepository;
        private readonly IRepository<Enrollment, int> _enrollmentRepository;
        private readonly IRepository<Coupon, int> _couponRepository;
        private readonly IRepository<CourseLanguage, int> _courseLanguageRepository;
        private readonly IRepository<CourseTag, int> _courseTagRepository;
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<OrderDetail, int> _orderDetailRepository;
        private readonly IRepository<CourseClass, int> _courseClassRepository;
        private readonly ICourseClassService _courseClassService;
        private readonly IReviewService _reviewService;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<UserProfile, string> _userProfileRepository;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ICertificateService _certificateService;

        public CourseService(IRepository<Course, int> courseRepository, IReviewService reviewService, IElasticsearchService elasticsearchService, IMapper mapper, IDiscountService discountService, IRepository<CourseInstructor, int> courseInstructorRepository, IRepository<Enrollment, int> enrollment, UserManager<User> userManager, IRepository<CourseLanguage, int> courseLanguageRepository, IRepository<CourseTag, int> courseTagRepository, ICourseClassService courseClassService, IRepository<UserProfile, string> userProfileRepository, IRepository<Coupon, int> couponRepository, IRepository<Order, int> orderRepository, IRepository<OrderDetail, int> orderDetailRepository, ICloudinaryService cloudinaryService, ICertificateService certificateService, IRepository<CourseClass, int> courseClassRepository)
        {
            _courseRepository = courseRepository;
            _reviewService = reviewService;
            _elasticsearchService = elasticsearchService;
            _mapper = mapper;
            _discountService = discountService;
            _courseInstructorRepository = courseInstructorRepository;
            _enrollmentRepository = enrollment;
            _userManager = userManager;
            _courseLanguageRepository = courseLanguageRepository;
            _courseTagRepository = courseTagRepository;
            _courseClassService = courseClassService;
            _userProfileRepository = userProfileRepository;
            _couponRepository = couponRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _cloudinaryService = cloudinaryService;
            _certificateService = certificateService;
            _courseClassRepository = courseClassRepository;
        }

        public async Task<Course?> GetCourse(int courseId)
        {
            try
            {
                return await _courseRepository.GetByIdAsync(courseId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {
            try
            {
                return await _courseRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course: " + ex.Message);
            }
        }

        public async Task AddCourse(Course course)
        {
            try
            {
                await _courseRepository.AddAsync(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the course: " + ex.Message);
            }
        }

        public async Task<ApiResponse> AddCourse(CreateCourseRequest req)
        {
            var tempFilePath1 = Path.GetTempFileName();
            var tempFilePath2 = Path.GetTempFileName();
            try
            {

                var instructor = await _userManager.FindByIdAsync(req.CreatedBy);

                if (instructor == null)
                {
                    throw new ArgumentException("Invalid instructor ID");
                }
                //add image, intro to cloudinary
                using (var stream = new FileStream(tempFilePath1, FileMode.Create))
                {
                    await req.ImageURL.CopyToAsync(stream);

                }
                using (var stream = new FileStream(tempFilePath2, FileMode.Create))
                {

                    await req.IntroURL.CopyToAsync(stream);
                }

                var introResponse = await _cloudinaryService.UploadVideoAsync(tempFilePath2, "vd-intro" + Guid.NewGuid());
                var imageResponse = await _cloudinaryService.UploadImageAsync(new UploadImageRequest() { File = req.ImageURL });
                var newCourse = new Course
                {
                    Title = req.Title,
                    ImageURL = imageResponse,
                    IntroURL = introResponse.VideoUri,
                    Description = req.Description,
                    Price = req.Price,
                    CreatedBy = req.CreatedBy,
                    DifficultyLevel = req.DifficultyLevel,
                    Prerequisites = req.Prerequisites,
                    LearningOutcomes = req.LearningOutcomes,
                    UpdatedBy = req.CreatedBy,

                    CourseInstructors = new List<CourseInstructor>
                    {
                        new CourseInstructor
                        {
                            InstructorId = instructor.Id,
                            IsPrimaryInstructor = true
                        }
                    }
                };

                await _courseRepository.AddAsync(newCourse);

                CreateCourseClassRequest createCourseClassRequest = new CreateCourseClassRequest()
                {
                    CourseId = newCourse.Id,
                    Title = req.Title,
                    ImageURL = imageResponse,
                    IntroURL = introResponse.VideoUri,
                    Description = req.Description,
                    Price = req.Price,
                    Duration = newCourse.Duration,
                    DifficultyLevel = req.DifficultyLevel,
                    Prerequisites = req.Prerequisites,
                    LearningOutcomes = newCourse.LearningOutcomes,
                    EstimatedCompletionTime = newCourse.EstimatedCompletionTime,
                    StartDate = DateTimeHelper.GetVietnamTime(),
                };

                await _courseClassService.AddCourseClass(createCourseClassRequest);

                await _certificateService.AddCertificate(new CreateCertificateRequest
                {
                    CourseId = newCourse.Id,
                });
                return new ApiResponse
                {
                    Data = new { CourseId = newCourse.Id },
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the course: " + ex.Message);
            }
        }

        public async Task UpdateCourse(UpdateCourseRequest req)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(req.CourseId);
                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }
                var instructor = await _userManager.FindByIdAsync(req.UpdatedBy);

                if (instructor == null)
                {
                    throw new ArgumentException("Invalid instructor ID");
                }
                var imageURI = course.ImageURL;

                if (req.ImageURL != null)
                {
                    var tempFilePath1 = Path.GetTempFileName();
                    using (var stream = new FileStream(tempFilePath1, FileMode.Create))
                    {
                        await req.ImageURL.CopyToAsync(stream);

                    }
                    imageURI = await _cloudinaryService.UploadImageAsync(new UploadImageRequest() { File = req.ImageURL });
                }

                var introURI = course.IntroURL;

                if (req.IntroURL != null)
                {
                    var tempFilePath2 = Path.GetTempFileName();

                    using (var stream = new FileStream(tempFilePath2, FileMode.Create))
                    {

                        await req.IntroURL.CopyToAsync(stream);
                    }

                    var introResponse = await _cloudinaryService.UploadVideoAsync(tempFilePath2, "vd-intro" + Guid.NewGuid());
                    introURI = introResponse.VideoUri;
                }

                // check if the instructor has permission to update the course
                //var courseInstructorDbSet = await _courseInstructorRepository.GetDbSet();
                //var isCourseInstructor = await courseInstructorDbSet.AnyAsync(ci => ci.CourseId == req.CourseId && ci.InstructorId == instructor.Id);

                //if (!isCourseInstructor)
                //{
                //    throw new Exception("Instructor does not have permission to update the course.");
                //}

                course.Title = req.Title;
                course.ImageURL = imageURI;
                course.IntroURL = introURI;
                course.Description = req.Description;
                course.Price = req.Price;
                course.DifficultyLevel = req.DifficultyLevel;
                course.Prerequisites = req.Prerequisites;
                course.LearningOutcomes = req.LearningOutcomes;
                course.UpdatedAt = DateTimeHelper.GetVietnamTime();
                course.UpdatedBy = req.UpdatedBy;

                await _courseRepository.UpdateAsync(course);

                var courseClass = await _courseClassService.GetNewestCourseClass(course.Id);
                if (courseClass == null)
                {
                    throw new Exception("Course class not found.");
                }
                courseClass.EndDate = DateTimeHelper.GetVietnamTime();
                courseClass.IsDeleted = true;

                await _courseClassService.UpdateCourseClass(courseClass);

                CreateCourseClassRequest createCourseClassRequest = new CreateCourseClassRequest()
                {
                    CourseId = course.Id,
                    Title = req.Title,
                    ImageURL = imageURI,
                    IntroURL = introURI,
                    Description = req.Description,
                    Price = req.Price,
                    Duration = course.Duration,
                    DifficultyLevel = req.DifficultyLevel,
                    Prerequisites = req.Prerequisites,
                    LearningOutcomes = course.LearningOutcomes,
                    EstimatedCompletionTime = course.EstimatedCompletionTime,
                    StartDate = DateTimeHelper.GetVietnamTime(),
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the course: " + ex.Message);
            }
        }

        public async Task UpdateCourse(Course course)
        {
            try
            {
                await _courseRepository.UpdateAsync(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the course: " + ex.Message);
            }
        }

        public async Task DeleteCourse(Course course)
        {
            try
            {
                await _courseRepository.DeleteAsync(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the course: " + ex.Message);
            }
        }

        public async Task DeleteCourse(int courtId)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(courtId);

                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }

                course.IsDeleted = true;

                await _courseRepository.UpdateAsync(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the course: " + ex.Message);
            }
        }

        public async Task<decimal> CalculateEffectivePrice(int courseId)
        {
            try
            {
                var dbSet = await _courseRepository.GetDbSet();

                var course = await dbSet
                    .Include(c => c.CourseDiscounts)
                    .ThenInclude(cd => cd.Discount)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }

                var effectivePrice = course.Price;
                if (course.CourseDiscounts.Any())
                {
                    var maxDiscount = course.CourseDiscounts
                        .Where(d => d.Discount.IsActive &&
                                    d.Discount.StartDate <= DateTimeHelper.GetVietnamTime() &&
                                    d.Discount.EndDate >= DateTimeHelper.GetVietnamTime())
                        .Max(d => d.Discount.DiscountType == "Percentage"
                            ? course.Price * d.Discount.DiscountValue / 100
                            : d.Discount.DiscountValue);

                    effectivePrice -= maxDiscount;
                }

                return effectivePrice;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while calculating the effective price: " + ex.Message);
            }
        }

        public async Task<int?> GetMaxDiscountId(int courseId)
        {
            try
            {
                var dbSet = await _courseRepository.GetDbSet();

                var course = await dbSet
                    .Include(c => c.CourseDiscounts)
                    .ThenInclude(cd => cd.Discount)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }

                var maxDiscount = course.CourseDiscounts
                    .Where(d => d.Discount.IsActive &&
                                (d.Discount.StartDate == null || d.Discount.StartDate <= DateTimeHelper.GetVietnamTime()) &&
                                (d.Discount.EndDate == null || d.Discount.EndDate >= DateTimeHelper.GetVietnamTime()) &&
                                (d.Discount.MaxUsage == null || d.Discount.UsageCount == null || d.Discount.MaxUsage > d.Discount.UsageCount))
                    .OrderByDescending(d => d.Discount.DiscountType == "Percentage"
                        ? course.Price * d.Discount.DiscountValue / 100
                        : d.Discount.DiscountValue)
                    .FirstOrDefault();

                return maxDiscount?.DiscountId;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the max discount ID: " + ex.Message);
            }
        }

        public async Task<List<CourseDTO>?> GetCoursesByConditions(GetCoursesRequest request)
        {
            try
            {
                var dbSet = await _courseRepository.GetDbSet();

                if (request.IsDeleted != null)
                {
                    dbSet = dbSet.Where(c => c.IsDeleted == request.IsDeleted);
                }

                if (request.IsPublished != null)
                {
                    dbSet = dbSet.Where(c => c.IsPublished == request.IsPublished);
                }

                if (request.TagId != null)
                {
                    dbSet = dbSet.Where(c => c.CourseTags.Any(t => t.TagId == request.TagId));
                }

                if (request.InstructorId != null)
                {
                    dbSet = dbSet.Where(c => c.CourseInstructors.Any(t => t.InstructorId == request.InstructorId));
                }

                if (request.LanguageId != null)
                {
                    dbSet = dbSet.Where(c => c.CourseLanguages.Any(t => t.LanguageId == request.LanguageId));
                }

                if (request.MinRating != null)
                {
                    dbSet = dbSet.Where(c => c.Reviews.Average(r => r.Rating) >= request.MinRating);
                }

                if (request.MaxRating != null)
                {
                    dbSet = dbSet.Where(c => c.Reviews.Average(r => r.Rating) <= request.MaxRating);
                }

                if (request.MinDuration != null)
                {
                    dbSet = dbSet.Where(c => c.Duration >= request.MinDuration);
                }

                if (request.MaxDuration != null)
                {
                    dbSet = dbSet.Where(c => c.Duration <= request.MaxDuration);
                }

                if (request.HasQuizzes == true)
                {
                    dbSet = dbSet.Where(c => c.Sections.Any(s => s.Lectures.Any(l => l.Quizzes.Count != 0)));
                }

                if (request.DifficultyLevel != null)
                {
                    dbSet = dbSet.Where(c => c.DifficultyLevel == request.DifficultyLevel);
                }

                if (request.StudentId != null)
                {
                    var boughtCourseQuery = (await _orderRepository.GetDbSet())
                        .Where(o => o.UserId == request.StudentId && o.OrderStatus == "Completed")
                        .SelectMany(o => o.OrderDetails)
                        .Select(od => od.CourseId)
                        .Distinct();

                    dbSet = dbSet.Where(c => !boughtCourseQuery.Contains(c.Id));
                }

                if (request.IsFree == true)
                {
                    dbSet = dbSet.Where(c => c.Price == 0);
                }

                if (request.ApprovalStatus != null)
                {
                    dbSet = dbSet.Where(c => c.ApprovalStatus == request.ApprovalStatus);
                }

                var items = await dbSet.ToListAsync();

                if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
                {
                    items = items
                        .Select(c => new
                        {
                            Course = c,
                            EffectivePrice = CalculateEffectivePrice(c.Id).Result
                        })
                        .Where(x =>
                            (!request.MinPrice.HasValue || x.EffectivePrice >= request.MinPrice) &&
                            (!request.MaxPrice.HasValue || x.EffectivePrice <= request.MaxPrice)
                        )
                        .Select(x => x.Course)
                        .ToList();
                }

                //var items = await dbSet
                //        .Include(c => c.CourseDiscounts)
                //        .ThenInclude(cd => cd.Discount)
                //        .ToListAsync();

                //if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
                //{
                //    items = items
                //        .Select(c => new
                //        {
                //            Course = c,
                //            MaxDiscount = c.CourseDiscounts
                //                .Where(d => d.Discount.IsActive &&
                //                            d.Discount.StartDate <= DateTime.UtcNow &&
                //                            d.Discount.EndDate >= DateTime.UtcNow)
                //                .Select(d => d.Discount.DiscountType == "Percentage"
                //                    ? c.Price * d.Discount.DiscountValue / 100
                //                    : d.Discount.DiscountValue)
                //                .DefaultIfEmpty(0m) // Handle courses with no valid discounts
                //                .Max()
                //        })
                //        .Where(x =>
                //            (!request.MinPrice.HasValue || (x.Course.Price - x.MaxDiscount) >= request.MinPrice) &&
                //            (!request.MaxPrice.HasValue || (x.Course.Price - x.MaxDiscount) <= request.MaxPrice)
                //        )
                //        .Select(x => x.Course)
                //        .ToList();
                //}

                var courseDTOs = _mapper.Map<List<CourseDTO>>(items);

                if (!string.IsNullOrEmpty(request.Title))
                {
                    if (!await _elasticsearchService.IsAvailableAsync())
                    {
                        courseDTOs = courseDTOs
                            .Where(p => p.Title.ToLower().Contains(request.Title.ToLower()) || p.Description.ToLower().Contains(request.Title.ToLower()))
                            .ToList();
                    }
                    else
                    {
                        await _elasticsearchService.EnsureIndexExistsAsync("courses");
                        await _elasticsearchService.IndexCoursesAsync(courseDTOs);
                        courseDTOs = await _elasticsearchService.SearchCoursesByNameAsync(request.Title);
                    }
                }

                return courseDTOs;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courses: " + ex.Message);
            }
        }

        public async Task<DiscountInformation> DiscountInformationResponse(int courseId)
        {
            try
            {
                var discountId = await GetMaxDiscountId(courseId);
                var discount = discountId != null ? await _discountService.GetDiscount((int)discountId) : null;
                return _mapper.Map<DiscountInformation>(discount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the discount information: " + ex.Message);
            }
        }

        //public async Task<List<InstructorInformation>> InstructorInformation(int courseId)
        //{
        //    try
        //    {
        //        var instructorDbset = await _courseInstructorRepository.GetDbSet();
        //        var instructors = await instructorDbset
        //            .Where(ci => ci.CourseId == courseId)
        //            .Select(ci => ci.Instructor)
        //            .ToListAsync();


        //        return _mapper.Map<List<InstructorInformation>>(instructors);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while getting the instructor information: " + ex.Message);
        //    }
        //}

        public async Task<List<InstructorInformation>> InstructorInformation(int courseId)
        {
            try
            {
                var instructorDbset = await _courseInstructorRepository.GetDbSet();
                var userProfileDbset = await _userProfileRepository.GetDbSet();

                var instructors = await instructorDbset
                    .Where(ci => ci.CourseId == courseId)
                    .Join(userProfileDbset,
                          ci => ci.InstructorId,
                          up => up.Id,
                          (ci, up) => new InstructorInformation
                          {
                              Id = ci.InstructorId,
                              Fullname = up.Fullname ?? null,
                              UserName = ci.Instructor.UserName,
                              Email = ci.Instructor.Email,
                              ProfilePictureUrl = up.ProfilePictureUrl ?? null
                          })
                    .ToListAsync();

                return instructors;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the instructor information: " + ex.Message);
            }
        }

        public async Task<int> NumberOfEnrollments(int courseId)
        {
            try
            {
                var enrollmentDbset = await _enrollmentRepository.GetDbSet();
                return await enrollmentDbset
                    .Where(e => e.Id == courseId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the number of enrollments: " + ex.Message);
            }
        }

        public async Task<int> TotalLectures(int courseId)
        {
            try
            {
                var dbSet = await _courseRepository.GetDbSet();
                var course = await dbSet
                    .Include(c => c.Sections)
                    .ThenInclude(s => s.Lectures)
                    .FirstOrDefaultAsync(c => c.Id == courseId);
                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }
                return course.Sections.Sum(s => s.Lectures.Count);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the total number of lectures: " + ex.Message);
            }
        }

        public async Task<int> TotalInstructors(int courseId)
        {
            try
            {
                var dbSet = await _courseRepository.GetDbSet();
                var course = await dbSet
                    .Include(c => c.CourseInstructors)
                    .FirstOrDefaultAsync(c => c.Id == courseId);
                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }
                return course.CourseInstructors.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the total number of instructors: " + ex.Message);
            }
        }

        public async Task<List<CourseCardResponse>> GetCourseInformation(GetCoursesRequest request)
        {
            try
            {
                var courseCard = new List<CourseCardResponse>();
                var courses = await GetCoursesByConditions(request);

                if (courses == null || courses.Count == 0)
                {
                    return new List<CourseCardResponse>();
                }

                foreach (var course in courses)
                {
                    var discount = await DiscountInformationResponse(course.Id);

                    if (discount != null)
                    {
                        discount.CalculateDiscountAndPrice(course.Price);
                    }

                    var courseCardResponse = new CourseCardResponse
                    {
                        Course = _mapper.Map<CoursesResponse>(course),
                        Tags = await GetTagInformation(course.Id),
                        Review = await _reviewService.GetAverageRatingAndNumberOfRatings(course.Id),
                        Discount = discount,
                        Instructors = await InstructorInformation(course.Id),
                        Enrollment = new EnrollmentInformation
                        {
                            TotalEnrollments = await NumberOfEnrollments(course.Id)
                        }
                    };
                    courseCard.Add(courseCardResponse);
                }

                return courseCard;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course information: " + ex.Message);
            }
        }

        public async Task<PaginatedList<CourseCardResponse>> GetPagingCourseInformation(GetCoursesRequest request, Paging paging)
        {
            try
            {
                var courseCards = await GetCourseInformation(request);

                if (!paging.PageSize.HasValue || paging.PageSize <= 0)
                {
                    paging.PageSize = 10;
                }

                if (!paging.PageIndex.HasValue || paging.PageIndex <= 0)
                {
                    paging.PageIndex = 1;
                }

                var totalCount = courseCards.Count;
                var skip = (paging.PageIndex.Value - 1) * paging.PageSize.Value;
                var take = paging.PageSize.Value;

                var validSortOptions = new[] { "most_popular", "highest_rated", "newest" };
                if (string.IsNullOrEmpty(paging.Sort) || !validSortOptions.Contains(paging.Sort))
                {
                    paging.Sort = "most_popular";
                }

                courseCards = paging.Sort switch
                {
                    "most_popular" => courseCards.OrderByDescending(p => p.Enrollment.TotalEnrollments).ToList(),
                    "highest_rated" => courseCards.OrderByDescending(p => p.Review.AverageRating).ToList(),
                    "newest" => courseCards.OrderByDescending(p => p.Course.CreatedAt).ToList(),
                    _ => courseCards
                };

                var paginatedCourseCards = courseCards.Skip(skip).Take(take).ToList();

                return new PaginatedList<CourseCardResponse>(paginatedCourseCards, totalCount, paging.PageIndex.Value, paging.PageSize.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course information: " + ex.Message);
            }
        }

        public async Task<CouponInformation?> CouponInformation(int courseId, string? userId)
        {
            try
            {
                if (userId == null)
                {
                    return null;
                }

                var dbSet = await _couponRepository.GetDbSet();
                var currentDate = DateTimeHelper.GetVietnamTime();

                var coupon = await dbSet
                    .Where(c => c.IsActive
                                && (c.ExpiryDate == null || c.ExpiryDate >= currentDate)
                                && (c.StartDate == null || c.StartDate <= currentDate)
                                && (c.MaxUsage == null || c.MaxUsage > c.UsageCount)
                                && c.CourseCoupons.Any(cc => cc.Id == courseId)
                                && c.CourseCoupons.Any(cc => cc.UserCourseCoupons.Any(ucc => ucc.UserId == userId)))
                                .FirstOrDefaultAsync();

                if (coupon == null)
                {
                    return null;
                }

                return _mapper.Map<CouponInformation>(coupon);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the coupon information: " + ex.Message);
            }
        }

        public async Task<List<TagResponse>> GetTagInformation(int courseId)
        {
            try
            {
                var dbSet = await _courseTagRepository.GetDbSet();
                var tags = await dbSet
                    .Where(ct => ct.CourseId == courseId)
                    .Select(ct => ct.Tag)
                    .ToListAsync();

                return _mapper.Map<List<TagResponse>>(tags);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the tag information: " + ex.Message);
            }
        }

        public async Task<CartCourseInformation> GetCartCourseInformationAsync(int courseId)
        {
            try
            {
                var course = await GetCourse(courseId);
                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }

                var cartCourseInformation = _mapper.Map<CartCourseInformation>(course);

                cartCourseInformation.TotalLectures = await TotalLectures(courseId);

                return cartCourseInformation;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the cart course information: " + ex.Message);
            }
        }

        public async Task<CourseSectionInformation> GetCourseSectionDetailsById(int courseId)
        {
            try
            {
                var course = await (await _courseRepository.GetDbSet())
                                .Include(c => c.CourseLanguages)
                                    .ThenInclude(cl => cl.Language)
                                .Include(c => c.CourseTags)
                                    .ThenInclude(ct => ct.Tag)
                                .Include(c => c.Sections)
                                .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }

                var supportedlanguages = course.CourseLanguages.Select(cl => cl.Language);
                var tags = course.CourseTags.Select(cl => cl.Tag.Name).ToList();
                var sections = course.Sections.ToList();

                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }

                var courseDetails = _mapper.Map<CourseDetails>(course);

                courseDetails.Tags = tags;
                courseDetails.Languages = _mapper.Map<IEnumerable<SupportedLanguage>>(supportedlanguages);
                courseDetails.Instructors = await InstructorInformation(courseId);
                courseDetails.Review = await _reviewService.GetAverageRatingAndNumberOfRatings(courseId);
                courseDetails.Enrollment = new EnrollmentInformation
                {
                    TotalEnrollments = await NumberOfEnrollments(courseId)
                };

                CourseSectionInformation coursePage = new CourseSectionInformation()
                {
                    CourseDetails = courseDetails,
                    SectionDetails = _mapper.Map<List<SectionDetails>>(sections)
                };

                return coursePage;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course details: " + ex.Message);
            }
        }

        public async Task<List<CourseCardResponse>> GetPersonalItemRecommendation(string? userId, int numberOfCourses)
        {
            try
            {
                var item = await PredictHybridRecommendationsV2(userId, 7, numberOfCourses);

                if (item == null || item.Count == 0)
                {
                    return await GetCourseInformation(new GetCoursesRequest());
                }

                var courseIds = item.OrderByDescending(c => c.Score).Select(c => c.CourseId).ToList();

                var courseCard = new List<CourseCardResponse>();

                foreach (var courseId in courseIds)
                {
                    var course = await GetCourse(courseId);
                    if (course == null)
                    {
                        throw new ArgumentException("Invalid course ID");
                    }
                    var discount = await DiscountInformationResponse(courseId);

                    if (discount != null)
                    {
                        discount.CalculateDiscountAndPrice(course.Price);
                    }

                    var courseCardResponse = new CourseCardResponse
                    {
                        Course = _mapper.Map<CoursesResponse>(course),
                        Tags = await GetTagInformation(course.Id),
                        Review = await _reviewService.GetAverageRatingAndNumberOfRatings(courseId),
                        Discount = discount,
                        Instructors = await InstructorInformation(courseId),
                        Enrollment = new EnrollmentInformation
                        {
                            TotalEnrollments = await NumberOfEnrollments(courseId)
                        }
                    };
                    courseCard.Add(courseCardResponse);
                }

                return courseCard;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course information: " + ex.Message);
            }
        }

        public async Task<CoursePage> GetCoursePageInformation(int courseId)
        {
            try
            {
                var courseSectionInformation = await GetCourseSectionDetailsById(courseId);
                var recommendedCourses = await GetItemDetailsThatStudentsAlsoBought(courseId);
                if (recommendedCourses == null || recommendedCourses.Count == 0)
                {
                    recommendedCourses = await GetCourseInformation(new GetCoursesRequest());
                    recommendedCourses = recommendedCourses.Take(10).ToList();
                }
                var ratingDetails = await _reviewService.GetRatingDetails(courseId);
                CoursePage coursePage = new CoursePage()
                {
                    CourseSectionInformation = courseSectionInformation,
                    RecommendedCourses = recommendedCourses,
                    RatingDetails = ratingDetails
                };
                return coursePage;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course page information: " + ex.Message);
            }
        }

        public async Task<List<CourseCardResponse>> GetItemDetailsThatStudentsAlsoBought(int courseId)
        {
            try
            {
                var courseCard = new List<CourseCardResponse>();
                var courseRecommendationV2 = await GetItemsThatStudentsAlsoBought(courseId);

                if (courseRecommendationV2 == null || courseRecommendationV2.Count == 0)
                {
                    return null;
                }
                var courseIds = courseRecommendationV2.OrderByDescending(c => c.Frequency).Take(10).Select(c => c.CourseId).AsQueryable();

                var courses = await (await _courseRepository.GetDbSet())
                    .Where(c => courseIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var course in courses)
                {
                    var discount = await DiscountInformationResponse(course.Id);

                    if (discount != null)
                    {
                        discount.CalculateDiscountAndPrice(course.Price);
                    }

                    var courseCardResponse = new CourseCardResponse
                    {
                        Course = _mapper.Map<CoursesResponse>(course),
                        Tags = await GetTagInformation(course.Id),
                        Review = await _reviewService.GetAverageRatingAndNumberOfRatings(course.Id),
                        Discount = discount,
                        Instructors = await InstructorInformation(course.Id),
                        Enrollment = new EnrollmentInformation
                        {
                            TotalEnrollments = await NumberOfEnrollments(course.Id)
                        }
                    };
                    courseCard.Add(courseCardResponse);
                }

                return courseCard;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course information: " + ex.Message);
            }
        }

        public async Task<List<CourseRecommendationV2>> GetItemsThatStudentsAlsoBought(int courseId)
        {
            try
            {
                var orderDbSet = await _orderRepository.GetDbSet();

                var orders = await orderDbSet
                    .Include(o => o.OrderDetails)
                    .Where(o => o.OrderStatus == "Completed" && o.OrderDetails.Any(od => od.CourseId == courseId))
                    .ToListAsync();

                Dictionary<int, int> courseFrequency = new Dictionary<int, int>();

                foreach (var order in orders)
                {
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        if (orderDetail.CourseId != courseId)
                        {
                            if (courseFrequency.ContainsKey(orderDetail.CourseId))
                            {
                                courseFrequency[orderDetail.CourseId]++;
                            }
                            else
                            {
                                courseFrequency[orderDetail.CourseId] = 1;
                            }
                        }
                    }
                }

                var courseRecommendations = new List<CourseRecommendationV2>();
                foreach (var course in courseFrequency)
                {
                    courseRecommendations.Add(new CourseRecommendationV2
                    {
                        CourseId = course.Key,
                        Frequency = course.Value
                    });
                }
                return courseRecommendations;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course information: " + ex.Message);
            }
        }

        public async Task<List<CourseRecommendation>> PredictHybridRecommendationsByRating(string userId)
        {
            try
            {
                double alpha = 0.5; // Weight for collaborative filtering score
                var mlContext = new MLContext();

                // Load collaborative filtering data
                var ratings = (await _reviewService.GetDbSetReview())
                    .Select(r => new UserCourseRating
                    {
                        UserId = r.UserId,
                        CourseId = r.Id,
                        Rating = r.Rating
                    })
                    .ToList();

                // Load data into IDataView
                IDataView dataView = mlContext.Data.LoadFromEnumerable(ratings);

                var pipeline = mlContext.Transforms.Conversion
                    .MapValueToKey(
                        inputColumnName: nameof(UserCourseRating.UserId),
                        outputColumnName: "UserIdKey")
                    .Append(mlContext.Transforms.Conversion.MapValueToKey(
                        inputColumnName: nameof(UserCourseRating.CourseId),
                        outputColumnName: "CourseIdKey"))
                    .Append(mlContext.Recommendation().Trainers.MatrixFactorization(
                        labelColumnName: nameof(UserCourseRating.Rating),
                        matrixColumnIndexColumnName: "UserIdKey",
                        matrixRowIndexColumnName: "CourseIdKey"));

                // Train the model
                var model = pipeline.Fit(dataView);

                // Create prediction engine
                var predictionEngine = mlContext.Model.CreatePredictionEngine<UserCourseRating, RatingPrediction>(model);

                // Prepare hybrid recommendations
                var recommendations = new List<CourseRecommendation>();
                var courseDbSet = await _courseRepository.GetDbSet();
                var courses = await courseDbSet.Where(p => p.IsPublished).ToListAsync();
                var courseVectors = await GetCourseFeatureVectors(courses);

                foreach (var course in courses)
                {
                    // Collaborative Filtering Score
                    var collaborativePrediction = predictionEngine.Predict(new UserCourseRating
                    {
                        UserId = userId,
                        CourseId = course.Id
                    });
                    var collaborativeScore = float.IsNaN(collaborativePrediction.Score) ? 0 : collaborativePrediction.Score;

                    // Content-Based Filtering Score
                    double contentScore = 0;
                    if (courseVectors.ContainsKey(course.Id))
                    {
                        foreach (var otherCourse in courses)
                        {
                            if (otherCourse.Id != course.Id)
                            {
                                var similarity = CalculateCosineSimilarity(
                                    courseVectors[course.Id],
                                    courseVectors[otherCourse.Id]);
                                contentScore += similarity;
                            }
                        }
                        contentScore /= courses.Count - 1; // Average similarity
                    }

                    // Hybrid Score
                    var hybridScore = alpha * collaborativeScore + (1 - alpha) * contentScore;

                    // Add to recommendations
                    recommendations.Add(new CourseRecommendation
                    {
                        CourseId = course.Id,
                        Score = (decimal)hybridScore
                    });
                }

                // Sort by score descending
                return recommendations.OrderByDescending(r => r.Score).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while predicting hybrid recommendations: " + ex.Message);
            }
        }

        public async Task<List<CourseRecommendation>> PredictRecommendationsByPersonalLatestOrder(string userId)
        {
            try
            {
                var orders = await _orderRepository.GetDbSet();
                var latestOrder = await orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefaultAsync();

                if (latestOrder == null)
                {
                    return new List<CourseRecommendation>();
                }

                var orderDetails = await _orderDetailRepository.GetDbSet();
                var orderDetailsInLatestOrder = await orderDetails
                    .Where(od => od.OrderId == latestOrder.Id)
                    .ToListAsync();
                var courseIdsInLatestOrder = orderDetailsInLatestOrder
                    .Select(od => od.CourseId)
                    .ToList();

                // Prepare hybrid recommendations
                var recommendations = new List<CourseRecommendation>();
                var courseDbSet = await _courseRepository.GetDbSet();
                var courses = await courseDbSet.Where(p => p.IsPublished).ToListAsync();
                var filteredCourses = courses
                    .Where(p => !courseIdsInLatestOrder.Contains(p.Id))
                    .ToList();

                var courseVectors = await GetCourseFeatureVectors(courses);

                foreach (var course in filteredCourses)
                {
                    double contentScore = 0;
                    if (courseVectors.ContainsKey(course.Id))
                    {
                        foreach (var courseId in courseIdsInLatestOrder)
                        {
                            var similarity = CalculateCosineSimilarity(
                                courseVectors[course.Id],
                                courseVectors[courseId]);
                            contentScore += similarity;

                        }
                        contentScore /= courses.Count - 1;
                    }

                    // Add to recommendations
                    recommendations.Add(new CourseRecommendation
                    {
                        CourseId = course.Id,
                        Score = (decimal)contentScore
                    });
                }

                // Sort by score descending
                return recommendations.OrderByDescending(r => r.Score).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while predicting recommendations by personal latest order: " + ex.Message);
            }
        }

        public async Task<List<int>> GetTrendingCourseInNumberOfPrviousDays(int days, int numberOfCourses)
        {
            try
            {
                var orders = await _orderRepository.GetDbSet();
                var orderDetails = await _orderDetailRepository.GetDbSet();
                var courses = await _courseRepository.GetDbSet();
                var startDate = DateTimeHelper.GetVietnamTime().AddDays(-days);
                var endDate = DateTimeHelper.GetVietnamTime();
                var trendingCourses = await orders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.OrderStatus == "Completed")
                    .Join(orderDetails, o => o.Id, od => od.OrderId, (o, od) => od)
                    .GroupBy(od => od.CourseId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .Take(numberOfCourses)
                    .ToListAsync();
                return trendingCourses;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting trending courses: " + ex.Message);
            }
        }

        //public async Task<List<CourseRecommendation>> PredictHybridRecommendations(string userId, int days, int numberOfCourses)
        //{
        //    try
        //    {
        //        double weightCF = 0.3; // Collaborative Filtering weight
        //        double weightCBF = 0.5; // Content-Based Filtering weight
        //        double weightTrending = 0.2; // Trending weight

        //        // Get CF Recommendations
        //        var collaborativeRecommendations = await PredictHybridRecommendationsByRating(userId);

        //        // Get CBF Recommendations from the latest order
        //        var contentBasedRecommendations = await PredictRecommendationsByPersonalLatestOrder(userId);

        //        // Get Trending Courses
        //        var trendingCourseTitles = await GetTrendingCourseInNumberOfPrviousDays(days, numberOfCourses);
        //        var trendingRecommendations = (await _courseRepository.GetDbSet())
        //            .Where(c => trendingCourseTitles.Contains(c.Title))
        //            .Select(c => new CourseRecommendation
        //            {
        //                CourseId = c.Id,
        //                Score = 1 // Assign equal score for all trending courses
        //            })
        //            .ToList();

        //        // Merge scores
        //        var recommendationDictionary = new Dictionary<int, double>();

        //        void AddOrUpdateScore(int courseId, double score, double weight)
        //        {
        //            if (!recommendationDictionary.ContainsKey(courseId))
        //                recommendationDictionary[courseId] = 0;

        //            recommendationDictionary[courseId] += score * weight;
        //        }

        //        // Add CF scores
        //        foreach (var rec in collaborativeRecommendations)
        //            AddOrUpdateScore(rec.CourseId, (double)rec.Score, weightCF);

        //        // Add CBF scores
        //        foreach (var rec in contentBasedRecommendations)
        //            AddOrUpdateScore(rec.CourseId, (double)rec.Score, weightCBF);

        //        // Add Trending scores
        //        foreach (var rec in trendingRecommendations)
        //            AddOrUpdateScore(rec.CourseId, (double)rec.Score, weightTrending);

        //        // Prepare final recommendations
        //        var finalRecommendations = recommendationDictionary
        //            .Select(kvp => new CourseRecommendation
        //            {
        //                CourseId = kvp.Key,
        //                Score = (decimal)kvp.Value
        //            })
        //            .OrderByDescending(r => r.Score)
        //            .Take(numberOfCourses) // Limit to requested number of courses
        //            .ToList();

        //        return finalRecommendations;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while predicting hybrid recommendations: " + ex.Message);
        //    }
        //}

        public async Task<List<CourseRecommendation>> PredictHybridRecommendationsV2(string? userId, int days, int numberOfCourses)
        {
            try
            {
                // Determine user type (cold or active)
                var isGuestOrColdUser = userId.IsNullOrEmpty() || await IsColdUser(userId);

                // Set weights dynamically
                double weightCF = isGuestOrColdUser ? 0.0 : 0.3;
                double weightCBF = isGuestOrColdUser ? 0.2 : 0.5;
                double weightTrending = isGuestOrColdUser ? 0.8 : 0.2;

                // Get recommendations
                var collaborativeRecommendations = isGuestOrColdUser
                    ? new List<CourseRecommendation>()
                    : await PredictHybridRecommendationsByRating(userId);

                var contentBasedRecommendations = isGuestOrColdUser
                    ? new List<CourseRecommendation>()
                    : await PredictRecommendationsByPersonalLatestOrder(userId);

                var trendingCourseIds = await GetTrendingCourseInNumberOfPrviousDays(days, numberOfCourses);
                var trendingRecommendations = new List<CourseRecommendation>();
                foreach (var courseId in trendingCourseIds)
                {
                    trendingRecommendations.Add(new CourseRecommendation
                    {
                        CourseId = courseId,
                        Score = 1 // Assign equal score for all trending courses
                    });
                }

                // Merge scores
                var recommendationDictionary = new Dictionary<int, double>();

                void AddOrUpdateScore(int courseId, double score, double weight)
                {
                    if (!recommendationDictionary.ContainsKey(courseId))
                        recommendationDictionary[courseId] = 0;

                    recommendationDictionary[courseId] += score * weight;
                }

                foreach (var rec in collaborativeRecommendations)
                    AddOrUpdateScore(rec.CourseId, (double)rec.Score, weightCF);

                foreach (var rec in contentBasedRecommendations)
                    AddOrUpdateScore(rec.CourseId, (double)rec.Score, weightCBF);

                foreach (var rec in trendingRecommendations)
                    AddOrUpdateScore(rec.CourseId, (double)rec.Score, weightTrending);

                // Prepare final recommendations
                var finalRecommendations = recommendationDictionary
                    .Select(kvp => new CourseRecommendation
                    {
                        CourseId = kvp.Key,
                        Score = (decimal)kvp.Value
                    })
                    .OrderByDescending(r => r.Score)
                    .Take(numberOfCourses)
                    .ToList();

                return finalRecommendations;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while predicting hybrid recommendations: {ex.Message}", ex);
            }
        }

        private async Task<bool> IsColdUser(string userId)
        {
            try
            {
                var orders = await _orderRepository.GetDbSet();
                var orderDetails = await _orderDetailRepository.GetDbSet();
                var courses = await _courseRepository.GetDbSet();
                var userOrders = orders
                    .Where(o => o.UserId == userId && o.OrderStatus == "Completed");
                var userCourseIds = await orderDetails
                    .Where(od => userOrders.Any(o => o.Id == od.OrderId))
                    .Select(od => od.CourseId)
                    .ToListAsync();
                var allCourseIds = await courses
                    .Select(c => c.Id)
                    .ToListAsync();
                return userCourseIds.Count < allCourseIds.Count / 2;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking if the user is cold: " + ex.Message);
            }
        }

        private async Task<Dictionary<int, float[]>> GetCourseFeatureVectors(IEnumerable<Course> courses)
        {
            try
            {
                // Step 1: Get a dictionary of course IDs and their associated languages
                var courseLanguages = (await _courseLanguageRepository.GetDbSet())
                                              .GroupBy(cl => cl.Id)
                                              .ToDictionary(
                                                  g => g.Key,
                                                  g => g.Select(cl => cl.LanguageId).ToHashSet()
                                              );

                // Step 2: Get a dictionary of course IDs and their associated tags (optional if needed)
                var courseTags = (await _courseTagRepository.GetDbSet())
                                        .GroupBy(ct => ct.Id)
                                        .ToDictionary(
                                            g => g.Key,
                                            g => g.Select(ct => ct.TagId).ToHashSet()
                                        );

                // Step 3: Build the feature vectors
                return courses.ToDictionary(course => course.Id, course =>
                {
                    // Check if this course shares at least one tag with any other course
                    var hasSharedTags = courses.Any(otherCourse =>
                        otherCourse.Id != course.Id &&
                        courseTags.ContainsKey(course.Id) &&
                        courseTags.ContainsKey(otherCourse.Id) &&
                        courseTags[course.Id].Overlaps(courseTags[otherCourse.Id]));

                    // Check if this course shares at least one language with any other course
                    var hasSharedLanguages = courses.Any(otherCourse =>
                        otherCourse.Id != course.Id &&
                        courseLanguages.ContainsKey(course.Id) &&
                        courseLanguages.ContainsKey(otherCourse.Id) &&
                        courseLanguages[course.Id].Overlaps(courseLanguages[otherCourse.Id]));

                    return new float[]
                    {
                    (float)course.Price / 1000, // Normalize price to a scale
                    (float)course.Duration / 100, // Normalize duration
                    course.DifficultyLevel == "Beginner" ? 1 : 0, // Binary representation of difficulty levels
                    course.DifficultyLevel == "Intermediate" ? 1 : 0,
                    course.DifficultyLevel == "Advanced" ? 1 : 0,
                    (float)course.EstimatedCompletionTime / 100, // Normalize estimated completion time
                    hasSharedTags ? 1 : 0, // 1 if shares at least one tag, otherwise 0
                    hasSharedLanguages ? 1 : 0 // 1 if shares at least one language, otherwise 0
                    };
                });
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting course feature vectors: " + ex.Message);
            }
        }

        private static double CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            try
            {
                double dotCourse = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
                double magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
                double magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));
                return dotCourse / (magnitudeA * magnitudeB);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while calculating cosine similarity: " + ex.Message);
            }
        }

        public async Task UpdateCourseDuration(int courseId)
        {
            try
            {
                var dbSet = await _courseRepository.GetDbSet();
                var course = await dbSet
                    .Include(c => c.Sections)
                    .FirstOrDefaultAsync(c => c.Id == courseId);
                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }
                course.Duration = (int)Math.Ceiling(course.Sections.Sum(s => s.Duration.TotalMinutes));
                await _courseRepository.UpdateAsync(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the course duration: " + ex.Message);
            }
        }

        public async Task<bool> HasStudentBoughtCourse(string studentId, int courseId)
        {
            var hasBought = await (await _orderRepository.GetDbSet())
                .Where(o => o.UserId == studentId && o.OrderStatus == "Completed")
                .SelectMany(o => o.OrderDetails)
                .AnyAsync(od => od.CourseId == courseId);

            return hasBought;
        }

        public async Task<List<CourseCompletionPercentageResponse>> GetCourseCompletionPercentage(GetCourseCompletionPercentage request)
        {
            try
            {
                var dbSet = await _courseRepository.GetDbSet();
                var query = dbSet.AsQueryable();

                if (request.CourseId.HasValue)
                {
                    query = query.Where(c => c.Id == request.CourseId.Value);
                }

                if (request.TagId.HasValue)
                {
                    query = query.Where(c => c.CourseTags.Any(ct => ct.TagId == request.TagId.Value));
                }

                if (!string.IsNullOrEmpty(request.InstructorId))
                {
                    query = query.Where(c => c.CourseInstructors.Any(ci => ci.InstructorId == request.InstructorId));
                }

                if (!string.IsNullOrEmpty(request.CourseName))
                {
                    query = query.Where(c => c.Title.Contains(request.CourseName));
                }

                var courses = await query.ToListAsync();
                var response = new List<CourseCompletionPercentageResponse>();

                foreach (var course in courses)
                {
                    var totalLectures = await TotalLectures(course.Id);
                    var completionPercentage = 0;

                    if (course.HasVideo == true) completionPercentage += 25;
                    if (course.HasQuiz == true) completionPercentage += 25;
                    if (course.HasDoc == true) completionPercentage += 25;
                    if (totalLectures >= course.HasAtLeastLecture) completionPercentage += 25;
                    //if (course.IsInstructorSpecialtyCourse == true) completionPercentage += 20;
                    //if (course.ApprovalStatus == "Approved") completionPercentage += 20;

                    response.Add(new CourseCompletionPercentageResponse
                    {
                        CourseId = course.Id,
                        CourseName = course.Title,
                        CourseImage = course.ImageURL,
                        CompletionPercentage = completionPercentage,
                        Status = course.ApprovalStatus,
                        CreatedAt = course.CreatedAt
                    });
                }

                if (request.IsCompleted.HasValue)
                {
                    response = response.Where(r => r.CompletionPercentage == (request.IsCompleted.Value ? 100 : 0)).ToList();
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course completion percentage: " + ex.Message);
            }
        }

        public async Task<PaginatedList<CourseCompletionPercentageResponse>> GetPagingCourseCompletionPercentage(GetCourseCompletionPercentage request, Paging paging)
        {
            try
            {
                var courseCompletionPercentage = await GetCourseCompletionPercentage(request);

                if (!paging.PageSize.HasValue || paging.PageSize <= 0)
                {
                    paging.PageSize = 10;
                }
                if (!paging.PageIndex.HasValue || paging.PageIndex <= 0)
                {
                    paging.PageIndex = 1;
                }

                var validSortOptions = new[] { "percentage", "newest" };
                if (string.IsNullOrEmpty(paging.Sort) || !validSortOptions.Contains(paging.Sort))
                {
                    paging.Sort = "newest";
                }

                // Apply sorting
                courseCompletionPercentage = paging.Sort switch
                {
                    "percentage" => paging.SortDirection == "desc"
                        ? courseCompletionPercentage.OrderByDescending(c => c.CompletionPercentage).ToList()
                        : courseCompletionPercentage.OrderBy(c => c.CompletionPercentage).ToList(),
                    "newest" => paging.SortDirection == "desc"
                        ? courseCompletionPercentage.OrderByDescending(c => c.CreatedAt).ToList()
                        : courseCompletionPercentage.OrderBy(c => c.CreatedAt).ToList(),
                    _ => courseCompletionPercentage
                };

                var totalCount = courseCompletionPercentage.Count;
                var skip = (paging.PageIndex.Value - 1) * paging.PageSize.Value;
                var take = paging.PageSize.Value;
                var paginatedResponse = courseCompletionPercentage.Skip(skip).Take(take).ToList();

                return new PaginatedList<CourseCompletionPercentageResponse>(paginatedResponse, totalCount, paging.PageIndex.Value, paging.PageSize.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course completion percentage: " + ex.Message);
            }
        }

        public async Task CheckAndUpdateCourseContent(int courseId)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }

                var dbSet = await _courseRepository.GetDbSet();
                var courseWithLectures = await dbSet
                    .Include(c => c.Sections)
                        .ThenInclude(s => s.Lectures)
                            .ThenInclude(s => s.Videos)
                    .Include(c => c.Sections)
                        .ThenInclude(s => s.Lectures)
                            .ThenInclude(s => s.Quizzes)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (courseWithLectures == null)
                {
                    throw new ArgumentException("Invalid course ID");
                }

                bool hasDoc = false;
                bool hasQuiz = false;
                bool hasVideo = false;
                int totalLectures = 0;

                foreach (var section in courseWithLectures.Sections)
                {
                    foreach (var lecture in section.Lectures)
                    {
                        if (lecture.LectureType == "Reading" && lecture.DocUrl != null)
                        {
                            hasDoc = true;
                        }
                        if (lecture.LectureType == "Quiz" && lecture.Quizzes.Count > 0)
                        {
                            hasQuiz = true;
                        }
                        if (lecture.LectureType == "Video" && lecture.Videos.Count > 0)
                        {
                            hasVideo = true;
                        }
                        totalLectures++;
                    }
                }

                course.HasDoc = hasDoc;
                course.HasQuiz = hasQuiz;
                course.HasVideo = hasVideo;

                await _courseRepository.UpdateAsync(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking and updating the course content: " + ex.Message);
            }
        }

        public async Task<List<Top5BestSellingCoursesResponse>> GetTop5BestSellingCourses()
        {
            try
            {
                var orderDetailsDbSet = await _orderDetailRepository.GetDbSet();
               var top5BestSellingCourseraw = orderDetailsDbSet.Include(c => c.Course)
                    .Where(od => od.Order.OrderStatus == "Completed")
                    .GroupBy(od => od.Course)
                    .Select(g => new
                    {
                        CourseId = g.Key.Id,
                        Title = g.Key.Title,
                        ImageURL = g.Key.ImageURL,
                        Description = g.Key.Description,
                        TotalSales = g.Sum(x => x.Price),
                    })
                    .OrderByDescending(x => x.TotalSales)
                    .Take(5)
                    .ToList();
                var result = top5BestSellingCourseraw
                    .Select((x, index) => new Top5BestSellingCoursesResponse
                    {
                        Rank = index + 1,
                        Id = x.CourseId,
                        TotalSales = x.TotalSales,
                        Description = x.Description,
                        Title = x.Title,
                        ImageURL = x.ImageURL,
                    })
                    .ToList();
                return result;
            } 
            catch (Exception ex) 
            {
                throw new Exception("An error occurred while get top 5 best selling course: " + ex.Message);
            }
        }

        public async Task<List<TagEnrollmentCountResponse>> GetStudentCountByTagAsync()
        {
            var enrollmentDbSet = await _enrollmentRepository.GetDbSet();
            var courseClassDbSet = await _courseClassRepository.GetDbSet();
            var courseTagDbSet = await _courseTagRepository.GetDbSet();
            var courseDbSet = await _courseRepository.GetDbSet();
            var result = await enrollmentDbSet
            .Join(courseClassDbSet, e => e.CourseClassId, cc => cc.Id, (e, cc) => new { e, cc })
            .Join(courseDbSet, combined => combined.cc.CourseId, c => c.Id, (combined, c) => new { combined.e, c })
            .Join(courseTagDbSet, combined => combined.c.Id, ct => ct.CourseId, (combined, ct) => new { combined.e, ct })
            .GroupBy(combined => combined.ct.Tag.Name)
            .Select(group => new TagEnrollmentCountResponse
            {
                TagName = group.Key,
                StudentCount = group.Count()
            })
            .OrderByDescending(result => result.StudentCount)
            .ToListAsync();

            return result;
        }
    }
}
