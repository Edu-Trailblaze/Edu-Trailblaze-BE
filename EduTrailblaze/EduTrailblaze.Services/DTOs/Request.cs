using EduTrailblaze.Services.Helper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EduTrailblaze.Services.DTOs
{
    public class GetCoursesRequest
    {
        public string? InstructorId { get; set; } = null;
        public int? LanguageId { get; set; } = null;
        public int? TagId { get; set; } = null;
        public string? Title { get; set; } = null;
        public decimal? MinRating { get; set; } = null;
        public decimal? MaxRating { get; set; } = null;
        public decimal? MinPrice { get; set; } = null;
        public decimal? MaxPrice { get; set; } = null;
        public bool? IsFree { get; set; } = null;
        public int? MinDuration { get; set; } = null;
        public int? MaxDuration { get; set; } = null;
        public bool? HasQuizzes { get; set; } = null;
        public string? StudentId { get; set; } = null;
        public string? DifficultyLevel { get; set; } = null;
        public bool? IsPublished { get; set; } = null;
        public bool? IsDeleted { get; set; } = null;
    }

    public class GetCoursesRequestValidator : AbstractValidator<GetCoursesRequest>
    {
        public GetCoursesRequestValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.MinRating)
                .InclusiveBetween(0, 5).WithMessage("MinRating must be between 0 and 5")
                .When(x => x.MinRating.HasValue);

            RuleFor(x => x.MaxRating)
                .InclusiveBetween(0, 5).WithMessage("MaxRating must be between 0 and 5")
                .GreaterThanOrEqualTo(x => x.MinRating).WithMessage("MaxRating must be greater than or equal to MinRating")
                .When(x => x.MaxRating.HasValue);

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("MinPrice must be greater than or equal to 0")
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0).WithMessage("MaxPrice must be greater than or equal to 0")
                .GreaterThanOrEqualTo(x => x.MinPrice ?? 0).WithMessage("MaxPrice must be greater than or equal to MinPrice")
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
                .WithMessage("MinPrice must be less than or equal to MaxPrice")
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);

            RuleFor(x => x.MinDuration)
                .GreaterThanOrEqualTo(0).WithMessage("MinDuration must be greater than or equal to 0")
                .When(x => x.MinDuration.HasValue);

            RuleFor(x => x.MaxDuration)
                .GreaterThanOrEqualTo(0).WithMessage("MaxDuration must be greater than or equal to 0")
                .GreaterThanOrEqualTo(x => x.MinDuration ?? 0).WithMessage("MaxDuration must be greater than or equal to MinDuration")
                .When(x => x.MaxDuration.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinDuration.HasValue || !x.MaxDuration.HasValue || x.MinDuration <= x.MaxDuration)
                .WithMessage("MinDuration must be less than or equal to MaxDuration")
                .When(x => x.MinDuration.HasValue && x.MaxDuration.HasValue);

            RuleFor(x => x.DifficultyLevel)
                .MaximumLength(50).WithMessage("DifficultyLevel must be Beginner, Intermediate, Advanced")
                .When(x => !string.IsNullOrEmpty(x.DifficultyLevel));

            RuleFor(x => x.StudentId)
                .MaximumLength(50).WithMessage("StudentId cannot be longer than 50 characters")
                .When(x => !string.IsNullOrEmpty(x.StudentId));
        }
    }

    public class CreateCourseRequest
    {
        public string Title { get; set; }

        public IFormFile? ImageURL { get; set; }

        public IFormFile? IntroURL { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string DifficultyLevel { get; set; } // Beginner, Intermediate, Advanced

        public string CreatedBy { get; set; }

        public string? Prerequisites { get; set; }

        public List<string> LearningOutcomes { get; set; }
    }

    public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
    {
        public CreateCourseRequestValidator()
        {
            RuleFor(x => x.Title)
             .NotEmpty().WithMessage("Title is required")
             .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");

            RuleFor(x => x.ImageURL)
                .NotEmpty().WithMessage("ImageURL is required");



            RuleFor(x => x.IntroURL)
                .NotEmpty().WithMessage("IntroURL is required");


            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

            RuleFor(x => x.DifficultyLevel)
                .NotEmpty().WithMessage("DifficultyLevel is required")
                .Must(level => new[] { "Beginner", "Intermediate", "Advanced" }.Contains(level))
                .WithMessage("DifficultyLevel must be Beginner, Intermediate, or Advanced");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required");

            RuleFor(x => x.Prerequisites)
                .MaximumLength(1000).WithMessage("Prerequisites cannot be longer than 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Prerequisites));

            RuleFor(x => x.LearningOutcomes)
                .NotEmpty().WithMessage("LearningOutcomes is required");
        }
    }

    public class UpdateCourseRequest
    {
        public int CourseId { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public string IntroURL { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string DifficultyLevel { get; set; } // Beginner, Intermediate, Advanced

        public string? Prerequisites { get; set; }

        public List<string> LearningOutcomes { get; set; }

        public string UpdatedBy { get; set; }

        public bool IsPublished { get; set; }

        //public bool IsDeleted { get; set; }
    }

    public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
    {
        public UpdateCourseRequestValidator()
        {

            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");

            RuleFor(x => x.Title)
             .NotEmpty().WithMessage("Title is required")
             .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");

            RuleFor(x => x.ImageURL)
                .NotEmpty().WithMessage("ImageURL is required")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("ImageURL must be a valid URL");

            RuleFor(x => x.IntroURL)
                .NotEmpty().WithMessage("IntroURL is required")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("IntroURL must be a valid URL");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

            RuleFor(x => x.DifficultyLevel)
                .NotEmpty().WithMessage("DifficultyLevel is required")
                .Must(level => new[] { "Beginner", "Intermediate", "Advanced" }.Contains(level))
                .WithMessage("DifficultyLevel must be Beginner, Intermediate, or Advanced");

            RuleFor(x => x.UpdatedBy)
                .NotEmpty().WithMessage("UpdatedBy is required");

            RuleFor(x => x.Prerequisites)
                .MaximumLength(1000).WithMessage("Prerequisites cannot be longer than 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Prerequisites));

            RuleFor(x => x.LearningOutcomes)
                .NotEmpty().WithMessage("LearningOutcomes is required");
        }
    }

    public class CreateCourseClassRequest
    {
        public int CourseId { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public string IntroURL { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; } = 0;

        public string DifficultyLevel { get; set; } // Beginner, Intermediate, Advanced

        public List<string> LearningOutcomes { get; set; }

        public decimal EstimatedCompletionTime { get; set; }

        public string? Prerequisites { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class CreateCourseClassRequestValidator : AbstractValidator<CreateCourseClassRequest>
    {
        public CreateCourseClassRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");

            RuleFor(x => x.Title)
             .NotEmpty().WithMessage("Title is required")
             .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");

            RuleFor(x => x.ImageURL)
                .NotEmpty().WithMessage("ImageURL is required")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("ImageURL must be a valid URL");

            RuleFor(x => x.IntroURL)
                .NotEmpty().WithMessage("IntroURL is required")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("IntroURL must be a valid URL");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(0).WithMessage("Duration must be greater than or equal to 0");

            RuleFor(x => x.DifficultyLevel)
                .NotEmpty().WithMessage("DifficultyLevel is required")
                .Must(level => new[] { "Beginner", "Intermediate", "Advanced" }.Contains(level))
                .WithMessage("DifficultyLevel must be Beginner, Intermediate, or Advanced");

            RuleFor(x => x.Prerequisites)
                .MaximumLength(1000).WithMessage("Prerequisites cannot be longer than 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Prerequisites));

            RuleFor(x => x.LearningOutcomes)
                .NotEmpty().WithMessage("LearningOutcomes is required");

            RuleFor(x => x.EstimatedCompletionTime)
                .GreaterThanOrEqualTo(0).WithMessage("EstimatedCompletionTime must be greater than or equal to 0");

            RuleFor(x => x.StartDate)
                .GreaterThan(DateTimeHelper.GetVietnamTime()).WithMessage("StartDate must be greater than the current date");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("EndDate must be greater than StartDate");
        }
    }

    public class UpdateCourseClassRequest
    {
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public string IntroURL { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; } = 0;

        public string DifficultyLevel { get; set; } // Beginner, Intermediate, Advanced

        public List<string> LearningOutcomes { get; set; }

        public decimal EstimatedCompletionTime { get; set; }

        public string? Prerequisites { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class UpdateCourseClassRequestValidator : AbstractValidator<UpdateCourseClassRequest>
    {
        public UpdateCourseClassRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");

            RuleFor(x => x.Title)
             .NotEmpty().WithMessage("Title is required")
             .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");

            RuleFor(x => x.ImageURL)
                .NotEmpty().WithMessage("ImageURL is required")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("ImageURL must be a valid URL");

            RuleFor(x => x.IntroURL)
                .NotEmpty().WithMessage("IntroURL is required")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("IntroURL must be a valid URL");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(0).WithMessage("Duration must be greater than or equal to 0");

            RuleFor(x => x.DifficultyLevel)
                .NotEmpty().WithMessage("DifficultyLevel is required")
                .Must(level => new[] { "Beginner", "Intermediate", "Advanced" }.Contains(level))
                .WithMessage("DifficultyLevel must be Beginner, Intermediate, or Advanced");

            RuleFor(x => x.Prerequisites)
                .MaximumLength(1000).WithMessage("Prerequisites cannot be longer than 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Prerequisites));

            RuleFor(x => x.LearningOutcomes)
                .NotEmpty().WithMessage("LearningOutcomes is required");

            RuleFor(x => x.EstimatedCompletionTime)
                .GreaterThanOrEqualTo(0).WithMessage("EstimatedCompletionTime must be greater than or equal to 0");

            RuleFor(x => x.StartDate)
                .GreaterThan(DateTimeHelper.GetVietnamTime()).WithMessage("StartDate must be greater than the current date");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("EndDate must be greater than StartDate");
        }
    }

    public class CreateAnswerRequest
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class CreateAnswerRequestValidator : AbstractValidator<CreateAnswerRequest>
    {
        public CreateAnswerRequestValidator()
        {
            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("QuestionId is required");

            RuleFor(x => x.AnswerText)
                .NotEmpty().WithMessage("AnswerText is required");

            RuleFor(x => x.IsCorrect)
                .Must(value => value == true || value == false).WithMessage("IsCorrect is required");
        }
    }

    public class UpdateAnswerRequest
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class UpdateAnswerRequestValidator : AbstractValidator<UpdateAnswerRequest>
    {
        public UpdateAnswerRequestValidator()
        {
            RuleFor(x => x.AnswerId)
                .NotEmpty().WithMessage("AnswerId is required");

            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("QuestionId is required");

            RuleFor(x => x.AnswerText)
                .NotEmpty().WithMessage("AnswerText is required");

            RuleFor(x => x.IsCorrect)
                .NotEmpty().WithMessage("IsCorrect is required");
        }
    }

    public class CreateCertificateRequest
    {
        public int CourseId { get; set; }
        public string CertificateTemplateUrl { get; set; } = "https://firebasestorage.googleapis.com/v0/b/court-callers.appspot.com/o/EduTrailblaze%2FCertificate%20Template.jpg?alt=media&token=be334afc-7ff4-4d71-b013-e1be98dac73d";
    }

    public class CreateCertificateRequestValidator : AbstractValidator<CreateCertificateRequest>
    {
        public CreateCertificateRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.CertificateTemplateUrl)
                .NotEmpty().WithMessage("CertificateTemplateUrl is required");
        }
    }

    public class UpdateCertificateRequest
    {
        public int CertificateId { get; set; }
        public int CourseId { get; set; }
        public string CertificateTemplateUrl { get; set; }
    }

    public class UpdateCertificateRequestValidator : AbstractValidator<UpdateCertificateRequest>
    {
        public UpdateCertificateRequestValidator()
        {
            RuleFor(x => x.CertificateId)
                .NotEmpty().WithMessage("CertificateId is required");
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.CertificateTemplateUrl)
                .NotEmpty().WithMessage("CertificateTemplateUrl is required");
        }
    }

    public class CreateCouponRequest
    {
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageCount { get; set; }
        public int? MaxUsage { get; set; }
    }

    public class CreateCouponRequestValidator : AbstractValidator<CreateCouponRequest>
    {
        public CreateCouponRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(50).WithMessage("Code cannot be longer than 50 characters");
            RuleFor(x => x.DiscountType)
                .NotEmpty().WithMessage("DiscountType is required")
                .Must(type => new[] { "Percentage", "Value" }.Contains(type))
                .WithMessage("DiscountType must be Percentage or Value");
            RuleFor(x => x.DiscountValue)
                .NotEmpty().WithMessage("DiscountValue is required")
                .GreaterThanOrEqualTo(0).WithMessage("DiscountValue must be greater than or equal to 0");
            RuleFor(x => x.StartDate)
                .GreaterThan(DateTimeHelper.GetVietnamTime()).WithMessage("StartDate must be greater than the current date")
                .When(x => x.StartDate.HasValue);
            RuleFor(x => x.ExpiryDate)
                .GreaterThan(x => x.ExpiryDate.Value).WithMessage("EndDate must be greater than ExpiryDate")
                .When(x => x.StartDate.HasValue && x.ExpiryDate.HasValue);
            RuleFor(x => x.UsageCount)
                .GreaterThanOrEqualTo(0).WithMessage("UsageCount must be greater than or equal to 0");
            RuleFor(x => x.MaxUsage)
                .GreaterThanOrEqualTo(0).WithMessage("MaxUsage must be greater than 0");
        }
    }

    public class UpdateCouponRequest
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int? UsageCount { get; set; }
        public int? MaxUsage { get; set; }
    }

    public class UpdateCouponRequestValidator : AbstractValidator<UpdateCouponRequest>
    {
        public UpdateCouponRequestValidator()
        {
            RuleFor(x => x.CouponId)
                .NotEmpty().WithMessage("CouponId is required");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(50).WithMessage("Code cannot be longer than 50 characters");
            RuleFor(x => x.DiscountType)
                .NotEmpty().WithMessage("DiscountType is required")
                .Must(type => new[] { "Percentage", "Value" }.Contains(type))
                .WithMessage("DiscountType must be Percentage or Value");
            RuleFor(x => x.DiscountValue)
                .NotEmpty().WithMessage("DiscountValue is required")
                .GreaterThanOrEqualTo(0).WithMessage("DiscountValue must be greater than or equal to 0");
            RuleFor(x => x.StartDate)
                .GreaterThan(DateTimeHelper.GetVietnamTime()).WithMessage("StartDate must be greater than the current date")
                .When(x => x.StartDate.HasValue);
            RuleFor(x => x.ExpiryDate)
                .GreaterThan(x => x.ExpiryDate.Value).WithMessage("EndDate must be greater than ExpiryDate")
                .When(x => x.StartDate.HasValue && x.ExpiryDate.HasValue);
            RuleFor(x => x.UsageCount)
                .GreaterThanOrEqualTo(0).WithMessage("UsageCount must be greater than or equal to 0");
            RuleFor(x => x.MaxUsage)
                .GreaterThanOrEqualTo(0).WithMessage("MaxUsage must be greater than 0");
            RuleFor(x => x.IsActive)
                .NotEmpty().WithMessage("IsActive is required");
        }
    }

    public class CreateDiscountRequest
    {
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxUsage { get; set; }
        public int? UsageCount { get; set; }
    }

    public class CreateDiscountRequestValidator : AbstractValidator<CreateDiscountRequest>
    {
        public CreateDiscountRequestValidator()
        {
            RuleFor(x => x.DiscountType)
                .NotEmpty().WithMessage("DiscountType is required")
                .MaximumLength(50).WithMessage("DiscountType cannot be longer than 50 characters");
            RuleFor(x => x.DiscountValue)
                .NotEmpty().WithMessage("DiscountValue is required")
                .GreaterThanOrEqualTo(0).WithMessage("DiscountValue must be greater than or equal to 0");
            RuleFor(x => x.StartDate)
                .GreaterThan(DateTimeHelper.GetVietnamTime()).WithMessage("StartDate must be greater than the current date")
                .When(x => x.StartDate.HasValue);
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate.Value).WithMessage("EndDate must be greater than StartDate")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
            RuleFor(x => x.MaxUsage)
                .GreaterThanOrEqualTo(0).WithMessage("MaxUsage must be greater than 0");
            RuleFor(x => x.UsageCount)
                .GreaterThanOrEqualTo(0).WithMessage("UsageCount must be greater than or equal to 0");
        }
    }

    public class UpdateDiscountRequest
    {
        public int DiscountId { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public int? MaxUsage { get; set; }
        public int? UsageCount { get; set; }
    }

    public class UpdateDiscountRequestValidator : AbstractValidator<UpdateDiscountRequest>
    {
        public UpdateDiscountRequestValidator()
        {
            RuleFor(x => x.DiscountId)
                .NotEmpty().WithMessage("DiscountId is required");
            RuleFor(x => x.DiscountType)
                .NotEmpty().WithMessage("DiscountType is required")
                .MaximumLength(50).WithMessage("DiscountType cannot be longer than 50 characters");
            RuleFor(x => x.DiscountValue)
                .NotEmpty().WithMessage("DiscountValue is required")
                .GreaterThanOrEqualTo(0).WithMessage("DiscountValue must be greater than or equal to 0");
            RuleFor(x => x.StartDate)
                .GreaterThan(DateTimeHelper.GetVietnamTime()).WithMessage("StartDate must be greater than the current date")
                .When(x => x.StartDate.HasValue);
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate.Value).WithMessage("EndDate must be greater than StartDate")
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
            RuleFor(x => x.MaxUsage)
                .GreaterThanOrEqualTo(0).WithMessage("MaxUsage must be greater than 0");
            RuleFor(x => x.UsageCount)
                .GreaterThanOrEqualTo(0).WithMessage("UsageCount must be greater than or equal to 0");
        }
    }

    public class CreateLanguageRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class CreateLanguageRequestValidator : AbstractValidator<CreateLanguageRequest>
    {
        public CreateLanguageRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(10).WithMessage("Code cannot be longer than 10 characters");
        }
    }

    public class UpdateLanguageRequest
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class UpdateLanguageRequestValidator : AbstractValidator<UpdateLanguageRequest>
    {
        public UpdateLanguageRequestValidator()
        {
            RuleFor(x => x.LanguageId)
                .NotEmpty().WithMessage("LanguageId is required");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(10).WithMessage("Code cannot be longer than 10 characters");
        }
    }

    public class CreateLectureDetails
    {
        public int SectionId { get; set; }
        public string LectureType { get; set; } // Reading, Video, Quiz
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public int? Duration { get; set; }
        public UploadVideoRequest? Video { get; set; }
    }

    public class CreateLectureDetailsValidator : AbstractValidator<CreateLectureDetails>
    {
        public CreateLectureDetailsValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("SectionId is required");

            RuleFor(x => x.LectureType)
                .NotEmpty().WithMessage("LectureType is required")
                .Must(type => new[] { "Reading", "Video", "Quiz" }.Contains(type))
                .WithMessage("LectureType must be Reading, Video, or Quiz");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(0).WithMessage("Duration must be greater than or equal to 0 if LectureType is not Video")
                .When(x => x.LectureType != "Video");

            RuleFor(x => x.Video)
                .NotEmpty().WithMessage("Videos are required if LectureType is Video")
                .When(x => x.LectureType == "Video");

            RuleFor(x => x.Video)
                .Null().WithMessage("Videos should be null if LectureType is not Video")
                .When(x => x.LectureType != "Video");
        }
    }

    public class CreateLecture
    {
        public int SectionId { get; set; }
        public string LectureType { get; set; } // Reading, Video, Quiz
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public IFormFile? ContentFile { get; set; }
        public int? Duration { get; set; } = 15;
    }

    public class CreateLectureValidator : AbstractValidator<CreateLecture>
    {
        public CreateLectureValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("SectionId is required");

            RuleFor(x => x.LectureType)
                .NotEmpty().WithMessage("LectureType is required")
                .Must(type => new[] { "Reading", "Video", "Quiz" }.Contains(type))
                .WithMessage("LectureType must be Reading, Video, or Quiz");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(0).WithMessage("Duration must be greater than or equal to 0 if LectureType is not Video")
                .When(x => x.LectureType != "Video");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required");

            RuleFor(x => x.ContentFile)
                .Must(BeAValidFileType).WithMessage("ContentFile must be a .docx or .pdf file")
                .When(x => x.ContentFile != null);
        }

        private bool BeAValidFileType(IFormFile? file)
        {
            if (file == null)
            {
                return true;
            }

            var allowedExtensions = new[] { ".docx", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }

    public class CreateLectureRequest
    {
        public int SectionId { get; set; }
        public string LectureType { get; set; } // Reading, Video, Quiz
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public int? Duration { get; set; }
    }

    public class CreateLectureRequestValidator : AbstractValidator<CreateLectureRequest>
    {
        public CreateLectureRequestValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("SectionId is required");
            RuleFor(x => x.LectureType)
                .NotEmpty().WithMessage("LectureType is required")
                .Must(type => new[] { "Reading", "Video", "Quiz" }.Contains(type))
                .WithMessage("LectureType must be Reading, Video, or Quiz");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(0).WithMessage("Duration must be greater than or equal to 0");
        }
    }

    public class UpdateLectureRequest
    {
        public int LectureId { get; set; }
        public int SectionId { get; set; }
        public string LectureType { get; set; } // Reading, Video, Quiz
        public string Title { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
    }

    public class UpdateLectureRequestValidator : AbstractValidator<UpdateLectureRequest>
    {
        public UpdateLectureRequestValidator()
        {
            RuleFor(x => x.LectureId)
                .NotEmpty().WithMessage("LectureId is required");
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("SectionId is required");
            RuleFor(x => x.LectureType)
                .NotEmpty().WithMessage("LectureType is required")
                .Must(type => new[] { "Reading", "Video", "Quiz" }.Contains(type))
                .WithMessage("LectureType must be Reading, Video, or Quiz");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Duration)
                .NotEmpty().WithMessage("Duration is required")
                .GreaterThanOrEqualTo(0).WithMessage("Duration must be greater than or equal to 0");
        }
    }

    public class CreateEnrollRequest
    {
        public int CourseId { get; set; }
        public string StudentId { get; set; }
    }

    public class CreateEnrollRequestValidator : AbstractValidator<CreateEnrollRequest>
    {
        public CreateEnrollRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required");
        }
    }

    public class CreateNewsRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CreateNewsRequestValidator : AbstractValidator<CreateNewsRequest>
    {
        public CreateNewsRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required");
            RuleFor(x => x.ImageUrl)
                .MaximumLength(255).WithMessage("ImageUrl cannot be longer than 255 characters");
        }
    }

    public class UpdateNewsRequest
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
    }

    public class UpdateNewsRequestValidator : AbstractValidator<UpdateNewsRequest>
    {
        public UpdateNewsRequestValidator()
        {
            RuleFor(x => x.NewsId)
                .NotEmpty().WithMessage("NewsId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required");
            RuleFor(x => x.ImageUrl)
                .MaximumLength(255).WithMessage("ImageUrl cannot be longer than 255 characters");
        }
    }

    public class CreateNotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsGlobal { get; set; }
    }

    public class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
    {
        public CreateNotificationRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required");
            RuleFor(x => x.IsGlobal)
                .NotEmpty().WithMessage("IsGlobal is required");
        }
    }

    public class UpdateNotificationRequest
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateNotificationRequestValidator : AbstractValidator<UpdateNotificationRequest>
    {
        public UpdateNotificationRequestValidator()
        {
            RuleFor(x => x.NotificationId)
                .NotEmpty().WithMessage("NotificationId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters");
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required");
            RuleFor(x => x.IsGlobal)
                .NotEmpty().WithMessage("IsGlobal is required");
            RuleFor(x => x.IsActive)
                .NotEmpty().WithMessage("IsActive is required");
        }
    }

    public class CreateOrderRequest
    {
        public string UserId { get; set; }
        public decimal OrderAmount { get; set; }
    }

    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.OrderAmount)
                .NotEmpty().WithMessage("OrderAmount is required")
                .GreaterThanOrEqualTo(0).WithMessage("OrderAmount must be greater than or equal to 0");
        }
    }

    public class OrderDetailRequest
    {
        public int OrderId { get; set; }
        public int CourseId { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequest>
    {
        public OrderDetailRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required");
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required")
                .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");
        }
    }

    public class PlaceOrderRequest
    {
        public string UserId { get; set; }
        public string PaymentMethod { get; set; }
        public int? VoucherCode { get; set; }
    }

    public class PlaceOrderRequestValidator : AbstractValidator<PlaceOrderRequest>
    {
        public PlaceOrderRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("PaymentMethod is required")
                .Must(method => new[] { "VnPay", "MoMo", "PayPal", "SystemBalance" }.Contains(method))
                .WithMessage("PaymentMethod must be VnPay, MoMo, or PayPal");
        }
    }

    public class UpdateOrderRequest
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
    }

    public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
    {
        public UpdateOrderRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required");
            RuleFor(x => x.OrderStatus)
                .NotEmpty().WithMessage("OrderStatus is required")
                .Must(status => new[] { "Pending", "Processing", "Cancelled" }.Contains(status))
                .WithMessage("OrderStatus must be Pending, Processing, or Cancelled");
        }
    }

    public class CreatePaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentRequestValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required");
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThanOrEqualTo(0).WithMessage("Amount must be greater than or equal to 0");
            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("PaymentMethod is required")
                .Must(method => new[] { "VnPay", "MoMo", "PayPal", "SystemBalance" }.Contains(method))
                .WithMessage("PaymentMethod must be VnPay, MoMo, or PayPal");
        }
    }

    public class UpdatePaymentRequest
    {
        public int PaymentId { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class UpdatePaymentRequestValidator : AbstractValidator<UpdatePaymentRequest>
    {
        public UpdatePaymentRequestValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEmpty().WithMessage("PaymentId is required");
            RuleFor(x => x.PaymentStatus)
                .NotEmpty().WithMessage("PaymentStatus is required")
                .Must(status => new[] { "Success", "Failed", "Pending" }.Contains(status))
                .WithMessage("PaymentStatus must be Success, Failed, or Pending");
        }
    }

    public class CreateQuestionRequest
    {
        public int QuizzId { get; set; }
        public string QuestionText { get; set; }
    }

    public class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
    {
        public CreateQuestionRequestValidator()
        {
            RuleFor(x => x.QuizzId)
                .NotEmpty().WithMessage("QuizzId is required");
            RuleFor(x => x.QuestionText)
                .NotEmpty().WithMessage("QuestionText is required");
        }
    }

    public class UpdateQuestionRequest
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
    }

    public class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>
    {
        public UpdateQuestionRequestValidator()
        {
            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("QuestionId is required");
            RuleFor(x => x.QuestionText)
                .NotEmpty().WithMessage("QuestionText is required");
        }
    }

    public class CreateQuizRequest
    {
        public int LectureId { get; set; }
        public string Title { get; set; }
        public decimal PassingScore { get; set; }
    }

    public class CreateQuizRequestValidator : AbstractValidator<CreateQuizRequest>
    {
        public CreateQuizRequestValidator()
        {
            RuleFor(x => x.LectureId)
                .NotEmpty().WithMessage("LectureId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot be longer than 200 characters");
            RuleFor(x => x.PassingScore)
                .NotEmpty().WithMessage("PassingScore is required")
                .GreaterThanOrEqualTo(0).WithMessage("PassingScore must be greater than or equal to 0");
        }
    }

    public class UpdateQuizRequest
    {
        public int QuizzId { get; set; }
        public string Title { get; set; }
        public decimal PassingScore { get; set; }
    }

    public class UpdateQuizRequestValidator : AbstractValidator<UpdateQuizRequest>
    {
        public UpdateQuizRequestValidator()
        {
            RuleFor(x => x.QuizzId)
                .NotEmpty().WithMessage("QuizzId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot be longer than 200 characters");
            RuleFor(x => x.PassingScore)
                .NotEmpty().WithMessage("PassingScore is required")
                .GreaterThanOrEqualTo(0).WithMessage("PassingScore must be greater than or equal to 0");
        }
    }

    public class CreateReviewRequest
    {
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public decimal Rating { get; set; }
        public string ReviewText { get; set; }
    }

    public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
    {
        public CreateReviewRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("Rating is required")
                .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5");
            RuleFor(x => x.ReviewText)
                .NotEmpty().WithMessage("ReviewText is required");
        }
    }

    public class UpdateReviewRequest
    {
        public int ReviewId { get; set; }
        public decimal Rating { get; set; }
        public string ReviewText { get; set; }
    }

    public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
    {
        public UpdateReviewRequestValidator()
        {
            RuleFor(x => x.ReviewId)
                .NotEmpty().WithMessage("ReviewId is required");
            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("Rating is required")
                .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5");
            RuleFor(x => x.ReviewText)
                .NotEmpty().WithMessage("ReviewText is required");
        }
    }

    public class CreateListSectionLectureRequest
    {
        public int CourseId { get; set; }
        public List<CreateSectionWithNoCourseRequest> Sections { get; set; }
    }
    public class CreateSectionWithNoCourseRequest
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public List<CreateLecture> Lectures { get; set; }
    }
    public class CreateSectionRequest
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public CreateLecture Lectures { get; set; }
    }

    public class CreateSectionRequestValidator : AbstractValidator<CreateSectionRequest>
    {
        public CreateSectionRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");
        }
    }

    public class UpdateSectionRequest
    {
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class UpdateSectionRequestValidator : AbstractValidator<UpdateSectionRequest>
    {
        public UpdateSectionRequestValidator()
        {
            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("SectionId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");
        }
    }

    public class CreateTagRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters");
            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Description cannot be longer than 200 characters");
        }
    }

    public class UpdateTagRequest
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateTagRequestValidator : AbstractValidator<UpdateTagRequest>
    {
        public UpdateTagRequestValidator()
        {
            RuleFor(x => x.TagId)
                .NotEmpty().WithMessage("TagId is required");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters");
            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Description cannot be longer than 200 characters");
        }
    }

    public class CreateQuizHistoryRequest
    {
        public string UserId { get; set; }
        public int QuizId { get; set; }
        public DateTime AttemptedOn { get; set; }
        public decimal Score { get; set; }
        public bool IsPassed { get; set; }
        public int DurationInSeconds { get; set; }
    }

    public class CreateQuizHistoryRequestValidator : AbstractValidator<CreateQuizHistoryRequest>
    {
        public CreateQuizHistoryRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.QuizId)
                .NotEmpty().WithMessage("QuizId is required");
            RuleFor(x => x.AttemptedOn)
                .NotEmpty().WithMessage("AttemptedOn is required");
            RuleFor(x => x.Score)
                .NotEmpty().WithMessage("Score is required")
                .GreaterThanOrEqualTo(0).WithMessage("Score must be greater than or equal to 0");
            RuleFor(x => x.DurationInSeconds)
                .NotEmpty().WithMessage("DurationInSeconds is required")
                .GreaterThanOrEqualTo(0).WithMessage("DurationInSeconds must be greater than or equal to 0");
        }
    }

    public class UpdateQuizHistoryRequest
    {
        public int QuizHistoryId { get; set; }
        public decimal Score { get; set; }
        public bool IsPassed { get; set; }
        public int DurationInSeconds { get; set; }
    }

    public class UpdateQuizHistoryRequestValidator : AbstractValidator<UpdateQuizHistoryRequest>
    {
        public UpdateQuizHistoryRequestValidator()
        {
            RuleFor(x => x.QuizHistoryId)
                .NotEmpty().WithMessage("QuizHistoryId is required ");
            RuleFor(x => x.Score)
                .NotEmpty().WithMessage("Score is required")
                .GreaterThanOrEqualTo(0).WithMessage("Score must be greater than or equal to 0");
            RuleFor(x => x.DurationInSeconds)
                .NotEmpty().WithMessage("DurationInSeconds is required")
                .GreaterThanOrEqualTo(0).WithMessage("DurationInSeconds must be greater than or equal to 0");
        }
    }

    public class CreateUserProfileRequest
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
    }

    public class CreateUserProfileRequestValidator : AbstractValidator<CreateUserProfileRequest>
    {
        public CreateUserProfileRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
        }
    }

    public class UpdateUserProfileRequest
    {
        //public string UserId { get; set; }
        public string FullName { get; set; }
        //public string ProfilePictureUrl { get; set; } = "https://firebasestorage.googleapis.com/v0/b/storage-8b808.appspot.com/o/OIP.jpeg?alt=media&token=60195a0a-2fd6-4c66-9e3a-0f7f80eb8473";
        public string PhoneNumber { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }

    public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
    {
        public UpdateUserProfileRequestValidator()
        {
            //RuleFor(x => x.UserId)
            //    .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.FullName)
                .MaximumLength(255).WithMessage("FullName cannot be longer than 255 characters");
            //RuleFor(x => x.ProfilePictureUrl)
            // .MaximumLength(255).WithMessage("ProfilePictureUrl cannot be longer than 255 characters")
            // .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("ProfilePictureUrl must be a valid URL");
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("PhoneNumber cannot be longer than 20 characters")
                .Matches(@"^\+?[0-9]\d{1,14}$").WithMessage("PhoneNumber must start with an optional '+' followed by 1 to 15 digits, and cannot contain any other characters.");
            RuleFor(x => x.ProfilePicture)
                .Must(BeAValidImage).WithMessage("ProfilePicture must be a valid image file (jpg, jpeg, png, gif)");
        }

        private bool BeAValidImage(IFormFile? file)
        {
            if (file == null)
            {
                return true;
            }

            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            return allowedContentTypes.Contains(file.ContentType);
        }
    }

    public class CreateVideoRequest
    {
        public int LectureId { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string Transcript { get; set; }
    }

    public class CreateVideoRequestValidator : AbstractValidator<CreateVideoRequest>
    {
        public CreateVideoRequestValidator()
        {
            RuleFor(x => x.LectureId)
                .NotEmpty().WithMessage("LectureId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.VideoUrl)
                .NotEmpty().WithMessage("VideoUrl is required");
            RuleFor(x => x.Transcript)
                .NotEmpty().WithMessage("Transcript is required");
        }
    }

    public class UpdateVideoRequest
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string Transcript { get; set; }
    }

    public class UpdateVideoRequestValidator : AbstractValidator<UpdateVideoRequest>
    {
        public UpdateVideoRequestValidator()
        {
            RuleFor(x => x.VideoId)
                .NotEmpty().WithMessage("VideoId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.VideoUrl)
                .NotEmpty().WithMessage("VideoUrl is required");
            RuleFor(x => x.Transcript)
                .NotEmpty().WithMessage("Transcript is required");
        }
    }

    public class CreateVoucherRequest
    {
        public string VoucherCode { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? MinimumOrderValue { get; set; }
    }

    public class CreateVoucherRequestValidator : AbstractValidator<CreateVoucherRequest>
    {
        public CreateVoucherRequestValidator()
        {
            RuleFor(x => x.VoucherCode)
                .NotEmpty().WithMessage("VoucherCode is required")
                .MaximumLength(50).WithMessage("VoucherCode cannot be longer than 50 characters");
            RuleFor(x => x.DiscountType)
                .NotEmpty().WithMessage("DiscountType is required")
                .MaximumLength(50).WithMessage("DiscountType cannot be longer than 50 characters");
            RuleFor(x => x.DiscountValue)
                .NotEmpty().WithMessage("DiscountValue is required")
                .GreaterThanOrEqualTo(0).WithMessage("DiscountValue must be greater than or equal to 0");
            RuleFor(x => x.StartDate)
                .GreaterThan(DateTimeHelper.GetVietnamTime()).WithMessage("StartDate must be greater than the current date")
                .When(x => x.StartDate.HasValue);
            RuleFor(x => x.ExpiryDate)
                .GreaterThan(x => x.StartDate.Value).WithMessage("ExpiryDate must be greater than StartDate")
                .When(x => x.StartDate.HasValue && x.ExpiryDate.HasValue);
            RuleFor(x => x.MinimumOrderValue)
                .GreaterThanOrEqualTo(0).WithMessage("MinimumOrderValue must be greater than or equal to 0");
        }
    }

    public class UpdateVoucherRequest
    {
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? MinimumOrderValue { get; set; }
        public bool IsUsed { get; set; }
    }

    public class UpdateVoucherRequestValidator : AbstractValidator<UpdateVoucherRequest>
    {
        public UpdateVoucherRequestValidator()
        {
            RuleFor(x => x.VoucherId)
                .NotEmpty().WithMessage("VoucherId is required");
            RuleFor(x => x.VoucherCode)
                .NotEmpty().WithMessage("VoucherCode is required")
                .MaximumLength(50).WithMessage("VoucherCode cannot be longer than 50 characters");
            RuleFor(x => x.DiscountType)
                .NotEmpty().WithMessage("DiscountType is required")
                .MaximumLength(50).WithMessage("DiscountType cannot be longer than 50 characters");
            RuleFor(x => x.DiscountValue)
                .NotEmpty().WithMessage("DiscountValue is required")
                .GreaterThanOrEqualTo(0).WithMessage("DiscountValue must be greater than or equal to 0");
            RuleFor(x => x.StartDate)
                .GreaterThan(DateTimeHelper.GetVietnamTime()).WithMessage("StartDate must be greater than the current date")
                .When(x => x.StartDate.HasValue);
            RuleFor(x => x.ExpiryDate)
                .GreaterThan(x => x.StartDate.Value).WithMessage("ExpiryDate must be greater than StartDate")
                .When(x => x.StartDate.HasValue && x.ExpiryDate.HasValue);
            RuleFor(x => x.MinimumOrderValue)
                .GreaterThanOrEqualTo(0).WithMessage("MinimumOrderValue must be greater than or equal to 0");
            RuleFor(x => x.IsUsed)
                .NotEmpty().WithMessage("IsUsed is required");
        }
    }

    public class DiscountInformation
    {
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal CalculatedDiscount { get; private set; }
        public decimal CalculatedPrice { get; private set; }

        public void CalculateDiscountAndPrice(decimal price)
        {
            CalculatedDiscount = DiscountType == "Percentage" ? price * DiscountValue / 100 : DiscountValue;
            CalculatedPrice = price - CalculatedDiscount;
        }
    }

    public class CouponInformation
    {
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal CalculatedDiscount { get; private set; }
        public decimal CalculatedPrice { get; private set; }

        public void CalculateDiscountAndPrice(decimal price)
        {
            CalculatedDiscount = DiscountType == "Percentage" ? price * DiscountValue / 100 : DiscountValue;
            CalculatedPrice = price - CalculatedDiscount;
        }
    }

    public class VoucherInformation
    {
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal CalculateDiscount(decimal price)
        {
            if (DiscountType == "Percentage")
            {
                return price * DiscountValue / 100;
            }
            return DiscountValue;
        }
        public decimal CalculatePrice(decimal price)
        {
            return price - CalculateDiscount(price);
        }
    }

    public class OrderItemInfomation
    {
        public string Title { get; set; }

        public string Instructor { get; set; }

        public string BasePrice { get; set; }

        public DiscountInformation DiscountInformation { get; set; }

        public CouponInformation CouponInformation { get; set; }
    }

    public class MailReceiptRequest
    {
        public string UserName { get; set; }
        public string InvoiceId { get; set; }
        public string OrderId { get; set; }
        public string OrderDate { get; set; }
        public string UserMail { get; set; }
        public string Source { get; set; } = "Edu Trailblaze";
        public List<OrderItemInfomation> OrderItems { get; set; }
        public VoucherInformation VoucherInformation { get; set; }
        public string TotalPrice { get; set; }
    }

    public class ApplyCouponRequest
    {
        public string CouponCode { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
    }

    public class RemoveCouponRequest
    {
        public string CouponCode { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
    }

    public class UploadVideoRequest
    {
        public IFormFile File { get; set; }
        public int LectureId { get; set; }
        public string Title { get; set; }
    }
    public class UploadImageRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Image is required")]
        public IFormFile File { get; set; }

    }


    public class UploadVideoRequestValidator : AbstractValidator<UploadVideoRequest>
    {
        public UploadVideoRequestValidator()
        {
            RuleFor(x => x.File)
                .NotEmpty().WithMessage("File is required");
            RuleFor(x => x.LectureId)
                .NotEmpty().WithMessage("LectureId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required");
        }
    }

    public class GetOrdersRequest
    {
        public string? UserId { get; set; }
        public string? OrderStatus { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxOrderAmount { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
    }

    public class GetOrdersRequestValidator : AbstractValidator<GetOrdersRequest>
    {
        public GetOrdersRequestValidator()
        {
            RuleFor(x => x.UserId)
                .MaximumLength(450).WithMessage("UserId cannot be longer than 450 characters");

            RuleFor(x => x.OrderStatus)
                .MaximumLength(50).WithMessage("OrderStatus cannot be longer than 50 characters");

            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("MinOrderAmount must be greater than or equal to 0");

            RuleFor(x => x.MaxOrderAmount)
                .GreaterThanOrEqualTo(0).WithMessage("MaxOrderAmount must be greater than or equal to 0");

            RuleFor(x => x)
            .Must(x => !x.MinOrderAmount.HasValue || !x.MaxOrderAmount.HasValue || x.MinOrderAmount <= x.MaxOrderAmount)
            .WithMessage("MinOrderAmount must be less than or equal to MaxOrderAmount")
            .When(x => x.MinOrderAmount.HasValue && x.MaxOrderAmount.HasValue);

            RuleFor(x => x.OrderDateFrom)
                .LessThanOrEqualTo(x => x.OrderDateTo.Value).WithMessage("OrderDateFrom must be less than or equal to OrderDateTo")
                .When(x => x.OrderDateFrom.HasValue && x.OrderDateTo.HasValue);

            RuleFor(x => x.OrderDateTo)
                .GreaterThanOrEqualTo(x => x.OrderDateFrom.Value).WithMessage("OrderDateTo must be greater than or equal to OrderDateFrom")
                .When(x => x.OrderDateFrom.HasValue && x.OrderDateTo.HasValue);
        }
    }

    public class CreateCourseInstructorRequest
    {
        public int CourseId { get; set; }
        public string InstructorId { get; set; }
        public bool IsPrimaryInstructor { get; set; }
    }

    public class CreateCourseInstructorRequestValidator : AbstractValidator<CreateCourseInstructorRequest>
    {
        public CreateCourseInstructorRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("InstructorId is required");
            RuleFor(x => x.IsPrimaryInstructor)
                .NotEmpty().WithMessage("IsPrimaryInstructor is required");
        }
    }

    public class InviteCourseInstructorRequest
    {
        public int CourseId { get; set; }
        public string InstructorId { get; set; }
    }

    public class InviteCourseInstructorRequestValidator : AbstractValidator<InviteCourseInstructorRequest>
    {
        public InviteCourseInstructorRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("InstructorId is required");
        }
    }

    public class RemoveCourseInstructorRequest
    {
        public int CourseId { get; set; }
        public string InstructorId { get; set; }
    }

    public class RemoveCourseInstructorRequestValidator : AbstractValidator<RemoveCourseInstructorRequest>
    {
        public RemoveCourseInstructorRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("InstructorId is required");
        }
    }

    public class GetVouchersRequest
    {
        public string? DiscountType { get; set; }

        public decimal? DiscountValueMin { get; set; }

        public decimal? DiscountValueMax { get; set; }

        public bool? IsUsed { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal? MinimumOrderValue { get; set; }

        public bool? IsValid { get; set; }
    }

    public class GetVouchersRequestValidator : AbstractValidator<GetVouchersRequest>
    {
        public GetVouchersRequestValidator()
        {
            RuleFor(x => x.DiscountType)
                .MaximumLength(50).WithMessage("DiscountType cannot be longer than 50 characters");
            RuleFor(x => x.DiscountValueMin)
                .GreaterThanOrEqualTo(0).WithMessage("DiscountValueMin must be greater than or equal to 0");
            RuleFor(x => x.DiscountValueMax)
                .GreaterThanOrEqualTo(0).WithMessage("DiscountValueMax must be greater than or equal to 0");
            RuleFor(x => x)
                .Must(x => !x.DiscountValueMin.HasValue || !x.DiscountValueMax.HasValue || x.DiscountValueMin <= x.DiscountValueMax)
                .WithMessage("DiscountValueMin must be less than or equal to DiscountValueMax")
                .When(x => x.DiscountValueMin.HasValue && x.DiscountValueMax.HasValue);
            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.ExpiryDate.Value).WithMessage("StartDate must be less than or equal to ExpiryDate")
                .When(x => x.StartDate.HasValue && x.ExpiryDate.HasValue);
            RuleFor(x => x.ExpiryDate)
                .GreaterThanOrEqualTo(x => x.StartDate.Value).WithMessage("ExpiryDate must be greater than or equal to StartDate")
                .When(x => x.StartDate.HasValue && x.ExpiryDate.HasValue);
            RuleFor(x => x.MinimumOrderValue)
                .GreaterThanOrEqualTo(0).WithMessage("MinimumOrderValue must be greater than or equal to 0");
        }
    }

    public class GetReviewsRequest
    {
        public int? CourseId { get; set; }
        public string? UserId { get; set; }
        public string? ReviewText { get; set; }
        public decimal? MinRating { get; set; }
        public decimal? MaxRating { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class GetReviewsRequestValidator : AbstractValidator<GetReviewsRequest>
    {
        public GetReviewsRequestValidator()
        {
            RuleFor(x => x.UserId)
                .MaximumLength(450).WithMessage("UserId cannot be longer than 450 characters");
            RuleFor(x => x.ReviewText)
                .MaximumLength(450).WithMessage("ReviewText cannot be longer than 450 characters");
            RuleFor(x => x.MinRating)
                .GreaterThanOrEqualTo(0).WithMessage("MinRating must be greater than or equal to 0");
            RuleFor(x => x.MaxRating)
                .GreaterThanOrEqualTo(0).WithMessage("MaxRating must be greater than or equal to 0");
            RuleFor(x => x)
                .Must(x => !x.MinRating.HasValue || !x.MaxRating.HasValue || x.MinRating <= x.MaxRating)
                .WithMessage("MinRating must be less than or equal to MaxRating")
                .When(x => x.MinRating.HasValue && x.MaxRating.HasValue);
        }
    }

    public class GetUsersRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public bool? LockoutEnabled { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
    }

    public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
    {
        public GetUsersRequestValidator()
        {
            RuleFor(x => x.UserName)
                .MaximumLength(450).WithMessage("UserName cannot be longer than 450 characters");
            RuleFor(x => x.Email)
                .MaximumLength(256).WithMessage("Email cannot be longer than 256 characters");
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50).WithMessage("PhoneNumber cannot be longer than 50 characters");
            RuleFor(x => x.FullName)
                .MaximumLength(255).WithMessage("FullName cannot be longer than 255 characters");
            RuleFor(x => x.Role)
                .MaximumLength(50).WithMessage("Role cannot be longer than 50 characters");
        }
    }

    public class GetSectionsRequest
    {
        public int? CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? MinNumberOfLectures { get; set; }
        public int? MaxNumberOfLectures { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
    }

    public class GetSectionsRequestValidator : AbstractValidator<GetSectionsRequest>
    {
        public GetSectionsRequestValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description cannot be longer than 255 characters");
            RuleFor(x => x)
                .Must(x => !x.MinNumberOfLectures.HasValue || !x.MaxNumberOfLectures.HasValue || x.MinNumberOfLectures <= x.MaxNumberOfLectures)
                .WithMessage("MinNumberOfLectures must be less than or equal to MaxNumberOfLectures")
                .When(x => x.MinNumberOfLectures.HasValue && x.MaxNumberOfLectures.HasValue);
            RuleFor(x => x)
                .Must(x => !x.MinDuration.HasValue || !x.MaxDuration.HasValue || x.MinDuration <= x.MaxDuration)
                .WithMessage("MinDuration must be less than or equal to MaxDuration")
                .When(x => x.MinDuration.HasValue && x.MaxDuration.HasValue);
            RuleFor(x => x.MinDuration)
                .GreaterThanOrEqualTo(0).WithMessage("MinDuration must be greater than or equal to 0");
            RuleFor(x => x.MaxDuration)
                .GreaterThanOrEqualTo(0).WithMessage("MaxDuration must be greater than or equal to 0");
            RuleFor(x => x.MinNumberOfLectures)
                .GreaterThanOrEqualTo(0).WithMessage("MinNumberOfLectures must be greater than or equal to 0");
            RuleFor(x => x.MaxNumberOfLectures)
                .GreaterThanOrEqualTo(0).WithMessage("MaxNumberOfLectures must be greater than or equal to 0");
        }
    }

    public class GetLecturesRequest
    {
        public int? SectionId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Description { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class GetLecturesRequestValidator : AbstractValidator<GetLecturesRequest>
    {
        public GetLecturesRequestValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.Content)
                .MaximumLength(255).WithMessage("Content cannot be longer than 255 characters");
            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Description cannot be longer than 255 characters");
            RuleFor(x => x)
                .Must(x => !x.MinDuration.HasValue || !x.MaxDuration.HasValue || x.MinDuration <= x.MaxDuration)
                .WithMessage("MinDuration must be less than or equal to MaxDuration")
                .When(x => x.MinDuration.HasValue && x.MaxDuration.HasValue);
            RuleFor(x => x.MinDuration)
                .GreaterThanOrEqualTo(0).WithMessage("MinDuration must be greater than or equal to 0");
            RuleFor(x => x.MaxDuration)
                .GreaterThanOrEqualTo(0).WithMessage("MaxDuration must be greater than or equal to 0");
        }
    }

    public class GetVideosRequest
    {
        public int? LectureId { get; set; }
        public string? Title { get; set; }
        public string? Transcript { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class GetVideosRequestValidator : AbstractValidator<GetVideosRequest>
    {
        public GetVideosRequestValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(50).WithMessage("Title cannot be longer than 50 characters");
            RuleFor(x => x.Transcript)
                .MaximumLength(255).WithMessage("Transcript cannot be longer than 255 characters");
            RuleFor(x => x)
                .Must(x => !x.MinDuration.HasValue || !x.MaxDuration.HasValue || x.MinDuration <= x.MaxDuration)
                .WithMessage("MinDuration must be less than or equal to MaxDuration")
                .When(x => x.MinDuration.HasValue && x.MaxDuration.HasValue);
            RuleFor(x => x.MinDuration)
                .GreaterThanOrEqualTo(0).WithMessage("MinDuration must be greater than or equal to 0");
            RuleFor(x => x.MaxDuration)
                .GreaterThanOrEqualTo(0).WithMessage("MaxDuration must be greater than or equal to 0");
        }
    }

    public class CreateCourseTagRequest
    {
        public int CourseId { get; set; }
        public int TagId { get; set; }
    }

    public class CreateCourseTagRequestValidator : AbstractValidator<CreateCourseTagRequest>
    {
        public CreateCourseTagRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.TagId)
                .NotEmpty().WithMessage("TagId is required");
        }
    }

    public class DeleteCourseTagRequest
    {
        public int CourseId { get; set; }
        public int TagId { get; set; }
    }

    public class DeleteCourseTagRequestValidator : AbstractValidator<DeleteCourseTagRequest>
    {
        public DeleteCourseTagRequestValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
            RuleFor(x => x.TagId)
                .NotEmpty().WithMessage("TagId is required");
        }
    }

    public class GetPaymentRequest
    {
        public int? OrderId { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? PaymentMethod { get; set; }
        //'VNPAY', 'MoMo', 'PayPal', 'SystemBalance'
        public string? PaymentStatus { get; set; }
        //'Success', 'Failed', 'Processing'
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetPaymentRequestValidator : AbstractValidator<GetPaymentRequest>
    {
        public GetPaymentRequestValidator()
        {
            RuleFor(x => x.MinAmount)
                .GreaterThanOrEqualTo(0).WithMessage("MinAmount must be greater than or equal to 0");
            RuleFor(x => x.MaxAmount)
                .GreaterThanOrEqualTo(0).WithMessage("MaxAmount must be greater than or equal to 0");
            RuleFor(x => x)
                .Must(x => !x.MinAmount.HasValue || !x.MaxAmount.HasValue || x.MinAmount <= x.MaxAmount)
                .WithMessage("MinAmount must be less than or equal to MaxAmount")
                .When(x => x.MinAmount.HasValue && x.MaxAmount.HasValue);
            RuleFor(x => x.PaymentMethod)
                .Must(status => string.IsNullOrEmpty(status) || new[] { "VNPAY", "MoMo", "PayPal", "SystemBalance" }.Contains(status))
                .WithMessage("PaymentMethod must be VNPAY, MoMo, PayPal, or SystemBalance");
            RuleFor(x => x.PaymentStatus)
                .Must(status => string.IsNullOrEmpty(status) || new[] { "Success", "Failed", "Processing" }.Contains(status))
                .WithMessage("PaymentStatus must be Success, Failed, or Processing");
            RuleFor(x => x.FromDate)
                .LessThanOrEqualTo(x => x.ToDate).WithMessage("FromDate must be less than or equal to ToDate")
                .When(x => x.FromDate != default && x.ToDate != default);
            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("ToDate must be greater than or equal to FromDate")
                .When(x => x.FromDate != default && x.ToDate != default);
        }
    }

    public class CreateQuizDetails
    {
        public int LectureId { get; set; }
        public string Title { get; set; }
        public decimal PassingScore { get; set; }
        public List<CreateQuestionDetails> Questions { get; set; }
    }

    public class CreateQuizDetailsValidator : AbstractValidator<CreateQuizDetails>
    {
        public CreateQuizDetailsValidator()
        {

            RuleFor(x => x.LectureId)
                .NotEmpty().WithMessage("LectureId is required");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot be longer than 200 characters");
            RuleFor(x => x.PassingScore)
                .NotEmpty().WithMessage("PassingScore is required")
                .GreaterThanOrEqualTo(0).WithMessage("PassingScore must be greater than or equal to 0");
            RuleFor(x => x.Questions)
                .NotEmpty().WithMessage("Questions is required")
                .Must(x => x.Count >= 1).WithMessage("Questions must have at least 1 item");
        }
    }

    public class CreateQuestionDetails
    {
        public string QuestionText { get; set; }
        public List<CreateAnswerDetails> Answers { get; set; }
    }

    public class CreateQuestionDetailsValidator : AbstractValidator<CreateQuestionDetails>
    {
        public CreateQuestionDetailsValidator()
        {
            RuleFor(x => x.QuestionText)
                .NotEmpty().WithMessage("QuestionText is required");
            RuleFor(x => x.Answers)
                .NotEmpty().WithMessage("Answers is required")
                .Must(x => x.Count >= 1).WithMessage("Answers must have at least 1 item");
        }
    }

    public class CreateAnswerDetails
    {
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class CreateAnswerDetailsValidator : AbstractValidator<CreateAnswerDetails>
    {
        public CreateAnswerDetailsValidator()
        {
            RuleFor(x => x.AnswerText)
                .NotEmpty().WithMessage("AnswerText is required");
            RuleFor(x => x.IsCorrect)
                .NotEmpty().WithMessage("IsCorrect is required");
        }
    }

    public class InstructorDashboardRequest
    {
        public string InstructorId { get; set; }
        public string Time { get; set; } // "week", "month", "year"
    }

    public class InstructorDashboardRequestValidator : AbstractValidator<InstructorDashboardRequest>
    {
        public InstructorDashboardRequestValidator()
        {
            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("InstructorId is required");
            RuleFor(x => x.Time)
                .NotEmpty().WithMessage("Time is required")
                .Must(x => new[] { "week", "month", "year" }.Contains(x))
                .WithMessage("Time must be week, month, or year");
        }
    }

    public class SaveUserProgressRequest
    {
        public string UserId { get; set; }
        public int LectureId { get; set; }
    }

    public class SaveUserProgressRequestValidator : AbstractValidator<SaveUserProgressRequest>
    {
        public SaveUserProgressRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.LectureId)
                .NotEmpty().WithMessage("LectureId is required");
        }
    }

    public class GetStudentCourses
    {
        public string StudentId { get; set; }
        public bool? IsEnrolled { get; set; }
    }

    public class GetStudentCoursesValidator : AbstractValidator<GetStudentCourses>
    {
        public GetStudentCoursesValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("UserId is required");
        }
    }

    public class ProgressRequest
    {
        public string UserId { get; set; }
        public int CourseClassId { get; set; }
        public int? SectionId { get; set; }
        public int? LectureId { get; set; }
        public int? QuizId { get; set; }
        public string? ProgressType { get; set; }
    }

    public class ProgressRequestValidator : AbstractValidator<ProgressRequest>
    {
        public ProgressRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.CourseClassId)
                .NotEmpty().WithMessage("CourseClassId is required");
            RuleFor(x => x)
                .Must(x => x.SectionId.HasValue || x.LectureId.HasValue || x.QuizId.HasValue)
                .WithMessage("Must have at least one of SectionId, LectureId, QuizId");
        }
    }

    public class UpdateUserProgress
    {
        public int Id { get; set; }
        public int? SectionId { get; set; }
        public int? LectureId { get; set; }
        public int? QuizId { get; set; }
        public decimal ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset LastAccessed { get; set; }
    }

    public class UpdateUserProgressValidator : AbstractValidator<UpdateUserProgress>
    {
        public UpdateUserProgressValidator()
        {
            RuleFor(x => x)
                .Must(x => x.SectionId.HasValue || x.LectureId.HasValue || x.QuizId.HasValue)
                .WithMessage("Must have at least one of SectionId, LectureId, QuizId");
            RuleFor(x => x.ProgressPercentage)
                .NotEmpty().WithMessage("ProgressPercentage is required")
                .GreaterThanOrEqualTo(0).WithMessage("ProgressPercentage must be greater than or equal to 0")
                .LessThanOrEqualTo(100).WithMessage("ProgressPercentage must be less than or equal to 100");
            RuleFor(x => x.LastAccessed)
                .NotEmpty().WithMessage("LastAccessed is required");
        }
    }

    public class CheckCourseStatusRequest
    {
        public string StudentId { get; set; }
        public int CourseId { get; set; }
    }

    public class CheckCourseStatusRequestValidator : AbstractValidator<CheckCourseStatusRequest>
    {
        public CheckCourseStatusRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required");
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
        }
    }

    public class GetStudentLearningCoursesRequest
    {
        public string StudentId { get; set; }
        public int? TagId { get; set; }
    }

    public class GetStudentLearningCoursesRequestValidator : AbstractValidator<GetStudentLearningCoursesRequest>
    {
        public GetStudentLearningCoursesRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required");

            RuleFor(x => x.TagId)
                .GreaterThanOrEqualTo(0).WithMessage("TagId must be greater than or equal to 0");
        }
    }

    public class StudentCourseProgressRequest
    {
        public string StudentId { get; set; }
        public int CourseId { get; set; }
    }

    public class StudentCourseProgressRequestValidator : AbstractValidator<StudentCourseProgressRequest>
    {
        public StudentCourseProgressRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("StudentId is required");
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
        }
    }

    public class CreateUserCertificateRequest
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }
    }

    public class CreateUserCertificateRequestValidator : AbstractValidator<CreateUserCertificateRequest>
    {
        public CreateUserCertificateRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("CourseId is required");
        }
    }

    public class GetCourseCertificatesRequest
    {
        public int? CourseId { get; set; }
        public string? UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetCourseCertificatesRequestValidator : AbstractValidator<GetCourseCertificatesRequest>
    {
        public GetCourseCertificatesRequestValidator()
        {
            RuleFor(x => x.FromDate)
                .LessThanOrEqualTo(x => x.ToDate).WithMessage("FromDate must be less than or equal to ToDate")
                .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
            RuleFor(x => x.ToDate)
                .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("ToDate must be greater than or equal to FromDate")
                .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
        }
    }

    public class CreateCourseCertificates
    {
        public string UserId { get; set; }
        public int CertificateId { get; set; }
        public string CertificateUrl { get; set; }
    }

    public class CreateCourseCertificatesValidator : AbstractValidator<CreateCourseCertificates>
    {
        public CreateCourseCertificatesValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.CertificateId)
                .NotEmpty().WithMessage("CertificateId is required");
            RuleFor(x => x.CertificateUrl)
                .NotEmpty().WithMessage("CertificateUrl is required");
        }
    }

    public class GetCourseCompletionPercentage
    {
        public int? CourseId { get; set; }
        public string? InstructorId { get; set; }
        public int? TagId { get; set; }
        public string? CourseName { get; set; }
        public bool? IsCompleted { get; set; }
    }

    public class GetCourseCompletionPercentageValidator : AbstractValidator<GetCourseCompletionPercentage>
    {
        public GetCourseCompletionPercentageValidator()
        {
            RuleFor(x => x.CourseName)
                .MaximumLength(255).WithMessage("CourseName cannot be longer than 255 characters");
            RuleFor(x => x.InstructorId)
                .MaximumLength(450).WithMessage("InstructorId cannot be longer than 450 characters");
        }
    }
}