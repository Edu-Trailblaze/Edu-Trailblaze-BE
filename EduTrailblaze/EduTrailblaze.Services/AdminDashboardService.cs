using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ICourseService _courseService;

        public AdminDashboardService(ICourseService courseService)
        {
            _courseService = courseService;
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

                course.ApprovalStatus = request.Status;
                await _courseService.UpdateCourse(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving the course: " + ex.Message);
            }
        }
    }
}
