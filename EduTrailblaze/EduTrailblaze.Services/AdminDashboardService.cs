using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using EduTrailblaze.Services.Helper;

namespace EduTrailblaze.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ICourseService _courseService;
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<Enrollment, int> _enrollmentRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AdminDashboardService(ICourseService courseService, UserManager<User> userManager, IRepository<Order, int> orderRepository, IMapper mapper, IRepository<Enrollment, int> enrollmentRepository)
        {
            _courseService = courseService;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task ApproveCourse(ApproveCourseRequest request)
        {
            try
            {
                var course = await _courseService.GetCourse(request.CourseId);

                if (course == null)
                {
                    throw new Exception("Course not found.");
                }

                if (course.ApprovalStatus != "Pending")
                {
                    throw new Exception("Course is not pending approval.");
                }

                course.ApprovalStatus = request.Status;
                course.IsPublished = request.Status == "Approved";
                await _courseService.UpdateCourse(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving the course: " + ex.Message);
            }
        }

        //public async Task ApproveCourseByAI(int courseId)
        //{
        //    try
        //    {
        //        var course = await _courseService.GetCourse(courseId);

        //        if (course == null)
        //        {
        //            throw new Exception("Course not found.");
        //        }

        //        var courseDetectAIRequest = new CourseDetectionRequest
        //        {
        //            title = course.Title,
        //            description = course.Description,
        //        };

        //        var tags = await _aiService.CourseDetectionAIV2(courseDetectAIRequest);

        //        if (tags == null || !tags.Any())
        //        {
        //            throw new Exception("AI response is null or empty.");
        //        }

        //        var tagDbSet = await _tagRepository.GetDbSet();
        //        var userTagDbSet = await _userTagRepository.GetDbSet();

        //        var hasTag = await userTagDbSet
        //            .Where(ut => ut.UserId == course.CreatedBy)
        //            .AnyAsync(ut => tags.AsQueryable().Contains(ut.Tag.Name));

        //        if (!hasTag)
        //        {
        //            course.IsPublished = false;
        //            course.ApprovalStatus = "Rejected";
        //        }
        //        else
        //        {
        //            course.IsPublished = true;
        //            course.ApprovalStatus = "Approved";
        //        }

        //        await _courseService.UpdateCourse(course);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while approving the course by AI: " + ex.Message);
        //    }
        //}

        public async Task<int> NumberOfStudents()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                return users.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the number of students in the system: " + ex.Message);
            }
        }

        public async Task<int> NumberOfInstructors()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync("Instructor");
                return users.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the number of instructors in the system: " + ex.Message);
            }
        }

        public async Task<decimal> TotalRevenue()
        {
            try
            {
                var orderDbSet = await _orderRepository.GetDbSet();
                var price = await orderDbSet
                    .Where(o => o.OrderStatus == "Completed")
                    .SumAsync(o => o.OrderAmount);
                return price;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the total revenue: " + ex.Message);
            }
        }

        public async Task<int> TotalCoursesBought()
        {
            try
            {
                var orderDbSet = await _orderRepository.GetDbSet();
                var courses = await orderDbSet
                    .Where(o => o.OrderStatus == "Completed")
                    .Select(o => o.OrderDetails)
                    .ToListAsync();
                return courses.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the total courses bought: " + ex.Message);
            }
        }

        public async Task<PaginatedList<CourseDTO>> GetPendingCourses(Paging paging)
        {
            try
            {
                var courses = await _courseService.GetCourses();
                var pendingCourses = courses.Where(c => c.ApprovalStatus == "Pending").ToList();

                var pendingCourseDTOs = _mapper.Map<List<Course>, List<CourseDTO>>(pendingCourses);

                if (!paging.PageSize.HasValue || paging.PageSize <= 0)
                {
                    paging.PageSize = 10;
                }

                if (!paging.PageIndex.HasValue || paging.PageIndex <= 0)
                {
                    paging.PageIndex = 1;
                }

                var totalCount = pendingCourseDTOs.Count;
                var skip = (paging.PageIndex.Value - 1) * paging.PageSize.Value;
                var take = paging.PageSize.Value;

                var validSortOptions = new[] { "title", "description", "oldest" };
                if (string.IsNullOrEmpty(paging.Sort) || !validSortOptions.Contains(paging.Sort))
                {
                    paging.Sort = "oldest";
                }

                pendingCourseDTOs = paging.Sort switch
                {
                    "title" => pendingCourseDTOs.OrderByDescending(p => p.Title).ToList(),
                    "description" => pendingCourseDTOs.OrderByDescending(p => p.Description).ToList(),
                    "oldest" => pendingCourseDTOs.OrderByDescending(p => p.CreatedAt).ToList(),
                    _ => pendingCourseDTOs
                };

                var paginatedPendingCourses = pendingCourseDTOs.Skip(skip).Take(take).ToList();

                return new PaginatedList<CourseDTO>(paginatedPendingCourses, totalCount, paging.PageIndex.Value, paging.PageSize.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the pending course: " + ex.Message);
            }
        }

        public async Task<CourseDataResponse> GetCourseData(CourseDataRequest request)
        {
            try
            {
                var query = await _orderRepository.GetDbSet();

                query = query.Where(o => o.OrderStatus == "Completed");

                if (request.CourseId.HasValue)
                {
                    query = query.Where(o => o.OrderDetails.Any(od => od.CourseId == request.CourseId.Value));
                }

                if (request.FromDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= request.FromDate.Value.ToDateTime(TimeOnly.MinValue));
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= request.ToDate.Value.ToDateTime(TimeOnly.MaxValue));
                }

                int numberOfCoursesPurchased;
                if (request.CourseId.HasValue)
                {
                    numberOfCoursesPurchased = await query
                        .SelectMany(o => o.OrderDetails)
                        .CountAsync(od => od.CourseId == request.CourseId.Value);
                }
                else
                {
                    numberOfCoursesPurchased = await query
                        .SelectMany(o => o.OrderDetails)
                        .CountAsync();
                }

                var totalRevenue = await query
                    .SumAsync(o => o.OrderAmount);

                return new CourseDataResponse
                {
                    NumberOfCoursesPurchased = numberOfCoursesPurchased,
                    TotalRevenue = totalRevenue
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course data: " + ex.Message);
            }
        }

        public async Task<List<ChartData>> GetNearestTimeForEnrollments(AdminDashboardRequest request)
        {
            try
            {
                List<ChartData> chartData = new List<ChartData>();
                var currentDate = DateTimeHelper.GetVietnamTime();

                if (request.Time == "week")
                {
                    var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                    var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                    for (int i = 0; i < 5; i++)
                    {
                        var weekStartDate = startOfWeek.AddDays(-7 * i);
                        var weekEndDate = i == 0 ? currentDate : endOfWeek.AddDays(-7 * i);

                        var enrollmentDbSet = await _enrollmentRepository.GetDbSet();
                        var enrollments = enrollmentDbSet
                            .Where(e => e.CreatedAt >= weekStartDate && e.CreatedAt <= weekEndDate)
                            .Count();

                        chartData.Add(new ChartData
                        {
                            FromDate = weekStartDate,
                            ToDate = weekEndDate,
                            Data = enrollments
                        });
                    }
                }
                else if (request.Time == "month")
                {
                    var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);
                    for (int i = 0; i < 5; i++)
                    {
                        var monthStartDate = startOfMonth.AddMonths(-1 * i);
                        var monthEndDate = i == 0 ? currentDate : endOfMonth.AddMonths(-1 * i);
                        var enrollmentDbSet = await _enrollmentRepository.GetDbSet();
                        var enrollments = enrollmentDbSet
                            .Where(e => e.CreatedAt >= monthStartDate && e.CreatedAt <= monthEndDate)
                            .Count();
                        chartData.Add(new ChartData
                        {
                            FromDate = monthStartDate,
                            ToDate = monthEndDate,
                            Data = enrollments
                        });
                    }
                }
                else if (request.Time == "year")
                {
                    var startOfYear = new DateTime(currentDate.Year, 1, 1);
                    var endOfYear = startOfYear.AddYears(1).AddSeconds(-1);
                    for (int i = 0; i < 5; i++)
                    {
                        var yearStartDate = startOfYear.AddYears(-1 * i);
                        var yearEndDate = i == 0 ? currentDate : endOfYear.AddYears(-1 * i);
                        var enrollmentDbSet = await _enrollmentRepository.GetDbSet();
                        var enrollments = enrollmentDbSet
                            .Where(e => e.CreatedAt >= yearStartDate && e.CreatedAt <= yearEndDate)
                            .Count();
                        chartData.Add(new ChartData
                        {
                            FromDate = yearStartDate,
                            ToDate = yearEndDate,
                            Data = enrollments
                        });
                    }
                }

                return chartData;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting nearest time for enrollments: " + ex.Message);
            }
        }

        public async Task<List<ChartData>> GetNearestTimeForRevenue(AdminDashboardRequest request)
        {
            try
            {
                List<ChartData> chartData = new List<ChartData>();
                var currentDate = DateTimeHelper.GetVietnamTime();
                if (request.Time == "week")
                {
                    var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                    var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);
                    for (int i = 0; i < 5; i++)
                    {
                        var weekStartDate = startOfWeek.AddDays(-7 * i);
                        var weekEndDate = i == 0 ? currentDate : endOfWeek.AddDays(-7 * i);
                        var orderDbSet = await _orderRepository.GetDbSet();
                        var totalRevenue = orderDbSet
                            .Where(o => o.OrderDate >= weekStartDate && o.OrderDate <= weekEndDate && o.OrderStatus == "Completed")
                            .SelectMany(o => o.OrderDetails)
                            .Sum(od => od.Price);
                        chartData.Add(new ChartData
                        {
                            FromDate = weekStartDate,
                            ToDate = weekEndDate,
                            Data = totalRevenue
                        });
                    }
                }
                else if (request.Time == "month")
                {
                    var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);
                    for (int i = 0; i < 5; i++)
                    {
                        var monthStartDate = startOfMonth.AddMonths(-1 * i);
                        var monthEndDate = i == 0 ? currentDate : endOfMonth.AddMonths(-1 * i);
                        var orderDbSet = await _orderRepository.GetDbSet();
                        var totalRevenue = orderDbSet
                            .Where(o => o.OrderDate >= monthStartDate && o.OrderDate <= monthEndDate && o.OrderStatus == "Completed")
                            .SelectMany(o => o.OrderDetails)
                            .Sum(od => od.Price);
                        chartData.Add(new ChartData
                        {
                            FromDate = monthStartDate,
                            ToDate = monthEndDate,
                            Data = totalRevenue
                        });
                    }
                }
                else if (request.Time == "year")
                {
                    var startOfYear = new DateTime(currentDate.Year, 1, 1);
                    var endOfYear = startOfYear.AddYears(1).AddSeconds(-1);
                    for (int i = 0; i < 5; i++)
                    {
                        var yearStartDate = startOfYear.AddYears(-1 * i);
                        var yearEndDate = i == 0 ? currentDate : endOfYear.AddYears(-1 * i);
                        var orderDbSet = await _orderRepository.GetDbSet();
                        var totalRevenue = orderDbSet
                            .Where(o => o.OrderDate >= yearStartDate && o.OrderDate <= yearEndDate && o.OrderStatus == "Completed")
                            .SelectMany(o => o.OrderDetails)
                            .Sum(od => od.Price);
                        chartData.Add(new ChartData
                        {
                            FromDate = yearStartDate,
                            ToDate = yearEndDate,
                            Data = totalRevenue
                        });
                    }
                }
                return chartData;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting nearest time for revenue: " + ex.Message);
            }
        }
    }
}
