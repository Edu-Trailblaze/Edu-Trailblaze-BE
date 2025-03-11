namespace EduTrailblaze.Services.DTOs
{
    public class CourseDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; } = 0;

        public string DifficultyLevel { get; set; }

        public string Prerequisites { get; set; }

        public List<string> LearningOutcomes { get; set; }

        public decimal EstimatedCompletionTime { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsPublished { get; set; }
        public bool? IsEnroll { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string ApprovalStatus { get; set; }
    }

    public class CartItemDTO
    {
        public int ItemId { get; set; }

        //public string ItemName { get; set; }

        //public decimal Price { get; set; }
    }

    public class OrderDTO
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public decimal OrderAmount { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; }
    }

    public class VoucherDTO
    {
        public int Id { get; set; }

        public string DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public string VoucherCode { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal? MinimumOrderValue { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class ReviewDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public decimal Rating { get; set; }
        public string ReviewText { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public string FullName { get; set; }
        public string[] Role { get; set; }
        public decimal Balance { get; set; }
        public string ProfilePictureUrl { get; set; }
    }

    public class SectionDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int NumberOfLectures { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class LectureDTO
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string? LectureType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class VideoDTO
    {
        public int Id { get; set; }
        public int LectureId { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string? Transcript { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class PaymentDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class QuizDTO
    {
        public int Id { get; set; }
        public int LectureId { get; set; }
        public string Title { get; set; }
        public decimal PassingScore { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
