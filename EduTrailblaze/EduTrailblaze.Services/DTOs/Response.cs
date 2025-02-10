namespace EduTrailblaze.Services.DTOs
{
    public class CoursesResponse
    {
        public int CourseId { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public string DifficultyLevel { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class DiscountInformationResponse
    {
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
    }

    public class CouponInformationResponse
    {
        public string CouponCode { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal PriceAfterDiscount { get; set; }
    }

    public class ReviewInformation
    {
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }

    public class InstructorInformation
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class EnrollmentInformation
    {
        public int TotalEnrollments { get; set; }
    }

    public class CourseCardResponse
    {
        public CoursesResponse Course { get; set; }
        public DiscountInformation? Discount { get; set; }
        public ReviewInformation Review { get; set; }
        public List<InstructorInformation> Instructors { get; set; }
        public EnrollmentInformation Enrollment { get; set; }
    }

    public class PaymentResponse
    {
        public bool IsSuccessful { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class CartInformation
    {
        public List<CartItemInformation> CartItems { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemInformation
    {
        public CartCourseInformation CartCourseInformation { get; set; }
        public List<InstructorInformation> InstructorInformation { get; set; }
        public ReviewInformation CourseReviewInformation { get; set; }
        public DiscountInformation? DiscountInformation { get; set; }
        public CouponInformation? CouponInformation { get; set; }
        public decimal TotalCoursePrice { get; set; }
    }

    public class CartCourseInformation
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public int Duration { get; set; }
        public string DifficultyLevel { get; set; }
        public int TotalLectures { get; set; }
    }

    public class UploadVideoResponse
    {
        public string VideoUri { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class ExchangeRateResponse
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }

    public class CourseRecommendation
    {
        public int CourseId { get; set; }
        public decimal Score { get; set; }
    }

    public class CourseRecommendationV2
    {
        public int CourseId { get; set; }
        public decimal Frequency { get; set; }
    }

    public class RatingPrediction
    {
        public float Score { get; set; }
    }

    public class UserCourseRating
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public decimal Rating { get; set; }
    }

    public class CourseDetails
    {
        public string Title { get; set; }

        public ReviewInformation Review { get; set; }

        public EnrollmentInformation Enrollment { get; set; }

        public List<InstructorInformation> Instructors { get; set; }

        public IEnumerable<SupportedLanguage> Languages { get; set; }

        public string ImageURL { get; set; }

        public string? IntroURL { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public string DifficultyLevel { get; set; }

        public List<string> LearningOutcomes { get; set; }

        public List<string> Skills { get; set; }

        public decimal EstimatedCompletionTime { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class SectionDetails
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int NumberOfLectures { get; set; }

        public TimeSpan Duration { get; set; }
    }

    public class CourseSectionInformation
    {
        public CourseDetails CourseDetails { get; set; }
        public List<SectionDetails> SectionDetails { get; set; }
    }

    public class CoursePage
    {
        public CourseSectionInformation CourseSectionInformation { get; set; }
        public List<CourseCardResponse> RecommendedCourses { get; set; }
    }

    public class SupportedLanguage
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}