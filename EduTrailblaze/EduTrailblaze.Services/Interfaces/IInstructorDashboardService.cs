using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IInstructorDashboardService
    {
        Task<DataDashboard> GetTotalCourses(string instructorId, string time);
        Task<DataDashboard> GetTotalEnrollments(string instructorId, string time);
        Task<DataDashboard> GetAverageRating(string instructorId, string time);
        Task<DataDashboard> GetTotalRevenue(string instructorId, string time);
    }
}
