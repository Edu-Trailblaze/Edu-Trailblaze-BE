using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Course?> GetCourse(int courseId);

        Task<IEnumerable<Course>> GetCourses();

        Task AddCourse(Course course);

        Task UpdateCourse(Course course);

        Task DeleteCourse(Course course);

        Task<List<CourseDTO>?> GetCoursesByConditions(GetCoursesRequest request);

        Task<decimal> CalculateEffectivePrice(int courseId);

        Task<int?> GetMaxDiscountId(int courseId);

        Task<List<CourseCardResponse>> GetCourseInformation(GetCoursesRequest request);

        Task<PaginatedList<CourseCardResponse>> GetPagingCourseInformation(GetCoursesRequest request, Paging paging);

        Task AddCourse(CreateCourseRequest course);

        Task UpdateCourse(UpdateCourseRequest req);

        Task DeleteCourse(int courtId);

        Task<int> NumberOfEnrollments(int courseId);

        Task<List<InstructorInformation>> InstructorInformation(int courseId);

        Task<DiscountInformation> DiscountInformationResponse(int courseId);

        Task<CouponInformation?> CouponInformation(int courseId, string? userId);

        Task<int> TotalLectures(int courseId);

        Task<int> TotalInstructors(int courseId);

        Task<CartCourseInformation> GetCartCourseInformationAsync(int courseId);

        Task<CoursePage> GetCoursePageInformation(int courseId);

        Task<CourseSectionInformation> GetCourseDetailsById(int courseId);

        Task<List<CourseCardResponse>> GetItemDetailsThatStudentsAlsoBought(int courseId);

        Task<List<CourseCardResponse>> GetPersonalItemRecommendation(string? userId);

        Task UpdateCourseDuration(int courseId);

        Task<List<TagResponse>> GetTagInformation(int courseId);
    }
}
