using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IInstructorDashboardService
    {
        Task<DataDashboard> GetTotalCourses(InstructorDashboardRequest request);
        Task<DataDashboard> GetTotalEnrollments(InstructorDashboardRequest request);
        Task<DataDashboard> GetAverageRating(InstructorDashboardRequest request);
        Task<DataDashboard> GetTotalRevenue(InstructorDashboardRequest request);
        Task<List<ChartData>> GetNearestTimeForRevenue(InstructorDashboardRequest request);
        Task<List<ChartData>> GetNearestTimeForEnrollments(InstructorDashboardRequest request);
    }
}
