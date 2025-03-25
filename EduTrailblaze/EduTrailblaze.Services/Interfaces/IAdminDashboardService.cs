using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IAdminDashboardService
    {
        Task ApproveCourse(ApproveCourseRequest request);

        Task ApproveCourseByAI(int courseId);

        Task<int> NumberOfInstructors();

        Task<int> NumberOfStudents();

        Task<decimal> TotalRevenue();

        Task<int> TotalCoursesBought();
    }
}
