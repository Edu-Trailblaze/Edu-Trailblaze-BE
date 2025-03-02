using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Entities;

namespace EduTrailblaze.Services
{
    public class InstructorDashboardService : IInstructorDashboardService
    {
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<Enrollment, int> _enrollmentRepository;
        private readonly IRepository<Review, int> _reviewRepository;
        private readonly IRepository<Order, int> _orderRepository;

        public InstructorDashboardService(IRepository<Course, int> courseRepository, IRepository<Enrollment, int> enrollmentRepository, IRepository<Review, int> reviewRepository, IRepository<Order, int> orderRepository)
        {
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
        }

        public async Task<DataDashboard> GetTotalCourses(string instructorId, string time)
        {
            try
            {
                DataDashboard dataDashboard = new DataDashboard();
                var courseDbSet = await _courseRepository.GetDbSet();

                var totalCourses = courseDbSet.Where(c => c.CreatedBy == instructorId);
                dataDashboard.CurrentData = totalCourses.Count();
                if (!string.IsNullOrEmpty(time))
                {
                    var currentTime = DateTime.UtcNow;
                    var startDate = time == "week" ? currentTime.AddDays(-7) : time == "month" ? currentTime.AddMonths(-1) : time == "year" ? currentTime.AddYears(-1) : currentTime;
                    totalCourses = totalCourses.Where(c => c.CreatedAt >= startDate);
                }

                dataDashboard.ComparisonData = totalCourses.Count();

                return dataDashboard;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting total courses: " + ex.Message);
            }
        }

        public async Task<DataDashboard> GetTotalEnrollments(string instructorId, string time)
        {
            try
            {
                DataDashboard dataDashboard = new DataDashboard();
                var enrollmentDbSet = await _enrollmentRepository.GetDbSet();
                var totalEnrollments = enrollmentDbSet.Where(e => e.CourseClass.Course.CreatedBy == instructorId);
                dataDashboard.CurrentData = totalEnrollments.Count();
                if (!string.IsNullOrEmpty(time))
                {
                    var currentTime = DateTime.UtcNow;
                    var startDate = time == "week" ? currentTime.AddDays(-7) : time == "month" ? currentTime.AddMonths(-1) : time == "year" ? currentTime.AddYears(-1) : currentTime;
                    totalEnrollments = totalEnrollments.Where(e => e.CreatedAt >= startDate);
                }
                dataDashboard.ComparisonData = totalEnrollments.Count();
                return dataDashboard;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting total enrollments: " + ex.Message);
            }
        }

        public async Task<DataDashboard> GetAverageRating(string instructorId, string time)
        {
            try
            {
                DataDashboard dataDashboard = new DataDashboard();
                var reviewDbSet = await _reviewRepository.GetDbSet();
                var totalReviews = reviewDbSet.Where(r => r.Course.CreatedBy == instructorId);
                dataDashboard.CurrentData = totalReviews.Count() > 0 ? totalReviews.Average(r => r.Rating) : 0;
                if (!string.IsNullOrEmpty(time))
                {
                    var currentTime = DateTime.UtcNow;
                    var startDate = time == "week" ? currentTime.AddDays(-7) : time == "month" ? currentTime.AddMonths(-1) : time == "year" ? currentTime.AddYears(-1) : currentTime;
                    totalReviews = totalReviews.Where(r => r.CreatedAt >= startDate);
                }
                dataDashboard.ComparisonData = totalReviews.Count() > 0 ? totalReviews.Average(r => r.Rating) : 0;
                return dataDashboard;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting average rating: " + ex.Message);
            }
        }

        //revenue all course
        public async Task<DataDashboard> GetTotalRevenue(string instructorId, string time)
        {
            try
            {
                DataDashboard dataDashboard = new DataDashboard();
                var orderDbSet = await _orderRepository.GetDbSet();
                var totalRevenue = orderDbSet.Where(o => o.OrderDetails.Any(od => od.Course.CreatedBy == instructorId)).Sum(o => o.OrderDetails.Sum(od => od.Price));
                dataDashboard.CurrentData = totalRevenue;
                if (!string.IsNullOrEmpty(time))
                {
                    var currentTime = DateTime.UtcNow;
                    var startDate = time == "week" ? currentTime.AddDays(-7) : time == "month" ? currentTime.AddMonths(-1) : time == "year" ? currentTime.AddYears(-1) : currentTime;
                    totalRevenue = orderDbSet.Where(o => o.OrderDetails.Any(od => od.Course.CreatedBy == instructorId && o.OrderDate >= startDate)).Sum(o => o.OrderDetails.Sum(od => od.Price));
                }
                dataDashboard.ComparisonData = totalRevenue;
                return dataDashboard;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting total revenue: " + ex.Message);
            }
        }
    }
}
