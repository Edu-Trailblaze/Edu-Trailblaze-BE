using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;

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

        public async Task<DataDashboard> GetTotalCourses(InstructorDashboardRequest request)
        {
            try
            {
                DataDashboard dataDashboard = new DataDashboard();
                var courseDbSet = await _courseRepository.GetDbSet();

                var totalCourses = courseDbSet.Where(c => c.CreatedBy == request.InstructorId);
                dataDashboard.CurrentData = totalCourses.Count();
                if (!string.IsNullOrEmpty(request.Time))
                {
                    var currentTime = DateTime.UtcNow;
                    var startDate = request.Time == "week" ? currentTime.AddDays(-7) : request.Time == "month" ? currentTime.AddMonths(-1) : request.Time == "year" ? currentTime.AddYears(-1) : currentTime;
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

        public async Task<DataDashboard> GetTotalEnrollments(InstructorDashboardRequest request)
        {
            try
            {
                DataDashboard dataDashboard = new DataDashboard();
                var enrollmentDbSet = await _enrollmentRepository.GetDbSet();
                var totalEnrollments = enrollmentDbSet.Where(e => e.CourseClass.Course.CreatedBy == request.InstructorId);
                dataDashboard.CurrentData = totalEnrollments.Count();
                if (!string.IsNullOrEmpty(request.Time))
                {
                    var currentTime = DateTime.UtcNow;
                    var startDate = request.Time == "week" ? currentTime.AddDays(-7) : request.Time == "month" ? currentTime.AddMonths(-1) : request.Time == "year" ? currentTime.AddYears(-1) : currentTime;
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

        public async Task<DataDashboard> GetAverageRating(InstructorDashboardRequest request)
        {
            try
            {
                DataDashboard dataDashboard = new DataDashboard();
                var reviewDbSet = await _reviewRepository.GetDbSet();
                var totalReviews = reviewDbSet.Where(r => r.Course.CreatedBy == request.InstructorId);
                dataDashboard.CurrentData = totalReviews.Count() > 0 ? totalReviews.Average(r => r.Rating) : 0;
                if (!string.IsNullOrEmpty(request.Time))
                {
                    var currentTime = DateTime.UtcNow;
                    var startDate = request.Time == "week" ? currentTime.AddDays(-7) : request.Time == "month" ? currentTime.AddMonths(-1) : request.Time == "year" ? currentTime.AddYears(-1) : currentTime;
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

        public async Task<DataDashboard> GetTotalRevenue(InstructorDashboardRequest request)
        {
            try
            {
                DataDashboard dataDashboard = new DataDashboard();
                var orderDbSet = await _orderRepository.GetDbSet();

                // Flatten the data and calculate the total revenue
                var totalRevenue = orderDbSet
                    .Where(o => o.OrderDetails.Any(od => od.Course.CreatedBy == request.InstructorId))
                    .SelectMany(o => o.OrderDetails)
                    .Where(od => od.Course.CreatedBy == request.InstructorId)
                    .Sum(od => od.Price);

                dataDashboard.CurrentData = totalRevenue;

                if (!string.IsNullOrEmpty(request.Time))
                {
                    var currentTime = DateTime.UtcNow;
                    var startDate = request.Time == "week" ? currentTime.AddDays(-7) : request.Time == "month" ? currentTime.AddMonths(-1) : request.Time == "year" ? currentTime.AddYears(-1) : currentTime;

                    // Flatten the data and calculate the total revenue for the specified time period
                    totalRevenue = orderDbSet
                        .Where(o => o.OrderDate >= startDate && o.OrderDetails.Any(od => od.Course.CreatedBy == request.InstructorId))
                        .SelectMany(o => o.OrderDetails)
                        .Where(od => od.Course.CreatedBy == request.InstructorId)
                        .Sum(od => od.Price);
                }

                dataDashboard.ComparisonData = totalRevenue;
                return dataDashboard;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting total revenue: " + ex.Message);
            }
        }

        public async Task<List<ChartData>> GetNearestTimeForEnrollments(InstructorDashboardRequest request)
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
                            .Where(e => e.CourseClass.Course.CreatedBy == request.InstructorId && e.CreatedAt >= weekStartDate && e.CreatedAt <= weekEndDate)
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
                            .Where(e => e.CourseClass.Course.CreatedBy == request.InstructorId && e.CreatedAt >= monthStartDate && e.CreatedAt <= monthEndDate)
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
                            .Where(e => e.CourseClass.Course.CreatedBy == request.InstructorId && e.CreatedAt >= yearStartDate && e.CreatedAt <= yearEndDate)
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

        public async Task<List<ChartData>> GetNearestTimeForRevenue(InstructorDashboardRequest request)
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
                            .Where(o => o.OrderDate >= weekStartDate && o.OrderDate <= weekEndDate && o.OrderDetails.Any(od => od.Course.CreatedBy == request.InstructorId))
                            .SelectMany(o => o.OrderDetails)
                            .Where(od => od.Course.CreatedBy == request.InstructorId)
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
                            .Where(o => o.OrderDate >= monthStartDate && o.OrderDate <= monthEndDate && o.OrderDetails.Any(od => od.Course.CreatedBy == request.InstructorId))
                            .SelectMany(o => o.OrderDetails)
                            .Where(od => od.Course.CreatedBy == request.InstructorId)
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
                            .Where(o => o.OrderDate >= yearStartDate && o.OrderDate <= yearEndDate && o.OrderDetails.Any(od => od.Course.CreatedBy == request.InstructorId))
                            .SelectMany(o => o.OrderDetails)
                            .Where(od => od.Course.CreatedBy == request.InstructorId)
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

        public async Task<decimal> GetCourseCompletionRate(string instructorId)
        {
            try
            {
                var enrollmentDbSet = await _enrollmentRepository.GetDbSet();
                var totalEnrollments = enrollmentDbSet.Where(e => e.CourseClass.Course.CreatedBy == instructorId).Count();
                var totalCompletedEnrollments = enrollmentDbSet.Where(e => e.CourseClass.Course.CreatedBy == instructorId && e.IsCompleted).Count();
                return totalEnrollments > 0 ? Math.Round((decimal)totalCompletedEnrollments / totalEnrollments * 100, 2) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting course completion rate: " + ex.Message);
            }
        }
    }
}
