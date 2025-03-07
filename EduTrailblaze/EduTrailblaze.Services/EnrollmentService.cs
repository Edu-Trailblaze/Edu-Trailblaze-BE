using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IRepository<Enrollment, int> _enrollmentRepository;
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<CourseClass, int> _courseClassRepository;

        public EnrollmentService(IRepository<Enrollment, int> enrollmentRepository, IRepository<Order, int> orderRepository, IRepository<CourseClass, int> courseClassRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _orderRepository = orderRepository;
            _courseClassRepository = courseClassRepository;
        }

        public async Task<Enrollment?> GetEnrollment(int enrollmentId)
        {
            try
            {
                return await _enrollmentRepository.GetByIdAsync(enrollmentId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the enrollment: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollments()
        {
            try
            {
                return await _enrollmentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the enrollment: " + ex.Message);
            }
        }

        public async Task EnrollCourse(CreateEnrollRequest enrollment)
        {
            try
            {
                var orderDbSet = await _orderRepository.GetDbSet();
                var order = await orderDbSet
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.UserId == enrollment.StudentId && o.OrderStatus == "Completed" && o.OrderDetails.Any(od => od.CourseId == enrollment.CourseId));

                if (order == null)
                {
                    throw new Exception("User has not bought the course.");
                }

                var courseClassIds = _courseClassRepository.FindByCondition(cc => cc.CourseId == enrollment.CourseId)
                    .Select(cc => cc.Id);

                var isEnrolled = await _enrollmentRepository.FindByCondition(e => e.StudentId == enrollment.StudentId && courseClassIds.Contains(e.CourseClassId))
                    .AnyAsync();

                if (isEnrolled)
                {
                    throw new Exception("User is already enrolled in a CourseClass of this Course.");
                }

                var newestCourseClass = await _courseClassRepository.FindByCondition(cc => cc.CourseId == enrollment.CourseId)
                    .OrderByDescending(cc => cc.CreatedAt)
                    .FirstOrDefaultAsync();

                if (newestCourseClass == null)
                {
                    throw new Exception("No CourseClass found for the given Course.");
                }

                var newEnrollment = new Enrollment
                {
                    StudentId = enrollment.StudentId,
                    CourseClassId = newestCourseClass.Id
                };

                await _enrollmentRepository.AddAsync(newEnrollment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the enrollment: " + ex.Message);
            }
        }

        public async Task UpdateEnrollment(Enrollment enrollment)
        {
            try
            {
                await _enrollmentRepository.UpdateAsync(enrollment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the enrollment: " + ex.Message);
            }
        }

        public async Task DeleteEnrollment(Enrollment enrollment)
        {
            try
            {
                await _enrollmentRepository.DeleteAsync(enrollment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the enrollment: " + ex.Message);
            }
        }

        public async Task<int> GetNumberOfStudentsEnrolledInCourse(int courseId)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetAllAsync();
                return enrollments.Count(e => e.Id == courseId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the number of students enrolled in the course: " + ex.Message);
            }
        }

        public async Task<List<StudentCourseResponse>> GetStudentCourses(GetStudentCourses request)
        {
            try
            {
                var boughtCourseIds = await (await _orderRepository.GetDbSet())
                    .Where(o => o.UserId == request.StudentId && o.OrderStatus == "Completed")
                    .SelectMany(o => o.OrderDetails)
                    .Select(od => od.CourseId)
                    .Distinct()
                    .ToListAsync();

                var enrollmentDbSet = await _enrollmentRepository.GetDbSet();
                var enrollments = await enrollmentDbSet.Where(e => e.StudentId == request.StudentId).ToListAsync();

                var response = new List<StudentCourseResponse>();

                foreach (var courseId in boughtCourseIds)
                {
                    try
                    {
                        var courseClassId = await GetStudentCourseClass(request.StudentId, courseId);
                        var isEnrolled = enrollments.Any(e => e.CourseClassId == courseClassId);

                        response.Add(new StudentCourseResponse
                        {
                            StudentId = request.StudentId,
                            CourseId = courseId.ToString(),
                            IsEnrolled = isEnrolled
                        });
                    }
                    catch (Exception)
                    {
                        response.Add(new StudentCourseResponse
                        {
                            StudentId = request.StudentId,
                            CourseId = courseId.ToString(),
                            IsEnrolled = false
                        });
                    }
                }

                if (request.IsEnrolled.HasValue)
                {
                    response = response.Where(r => r.IsEnrolled == request.IsEnrolled.Value).ToList();
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the number of courses enrolled by the student: " + ex.Message);
            }
        }

        public async Task<int> GetStudentCourseClass(string userId, int courseId)
        {
            try
            {
                var courseClassIds = _courseClassRepository.FindByCondition(cc => cc.CourseId == courseId)
                    .Select(cc => cc.Id);

                var enrollment = await _enrollmentRepository.FindByCondition(e => e.StudentId == userId && courseClassIds.Contains(e.CourseClassId))
                    .FirstOrDefaultAsync();
                if (enrollment == null)
                {
                    throw new Exception("Student is not enrolled in the CourseClass.");
                }

                return enrollment.CourseClassId;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the student's CourseClass: " + ex.Message);
            }
        }

        public async Task<CourseStatus> CheckCourseStatus(string studentId, string courseId)
        {
            try
            {
               var orderDbSet = await _orderRepository.GetDbSet();
                var hasBoughtCourse = await orderDbSet
                    .Include(o => o.OrderDetails)
                    .AnyAsync(o => o.UserId == studentId && o.OrderStatus == "Completed" && o.OrderDetails.Any(od => od.CourseId.ToString() == courseId));

                if (!hasBoughtCourse)
                {
                    return new CourseStatus
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        Status = "Not bought"
                    };
                }

                // Step 2: Check if the student is enrolled in the course
                var courseClassIds = _courseClassRepository.FindByCondition(cc => cc.CourseId.ToString() == courseId)
                    .Select(cc => cc.Id);

                var isEnrolled = await _enrollmentRepository.FindByCondition(e => e.StudentId == studentId && courseClassIds.Contains(e.CourseClassId))
                    .AnyAsync();

                if (isEnrolled)
                {
                    return new CourseStatus
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        Status = "Enrolled"
                    };
                }

                return new CourseStatus
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    Status = "Not enrolled"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking the course status: " + ex.Message);
            }
        }
    }
}
