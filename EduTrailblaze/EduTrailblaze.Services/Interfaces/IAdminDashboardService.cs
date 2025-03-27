using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IAdminDashboardService
    {
        Task ApproveCourse(ApproveCourseRequest request);

        //Task ApproveCourseByAI(int courseId);

        Task<int> NumberOfInstructors();

        Task<int> NumberOfStudents();

        Task<decimal> TotalRevenue();

        Task<int> TotalCoursesBought();

        Task<PaginatedList<CourseDTO>> GetPendingCourses(Paging paging);

        Task<CourseDataResponse> GetCourseData(CourseDataRequest request);

        Task<List<ChartData>> GetNearestTimeForEnrollments(AdminDashboardRequest request);

        Task<List<ChartData>> GetNearestTimeForRevenue(AdminDashboardRequest request);
    }
}
