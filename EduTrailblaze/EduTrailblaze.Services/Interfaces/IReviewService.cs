using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Review?> GetReview(int reviewId);

        Task<IEnumerable<Review>> GetReviews();

        Task AddReview(Review review);

        Task UpdateReview(Review review);

        Task DeleteReview(Review review);

        Task AddReview(CreateReviewRequest review);

        Task UpdateReview(UpdateReviewRequest review);

        Task DeleteReview(int review);

        Task<ReviewInformation> GetAverageRatingAndNumberOfRatings(int courseId);

        Task<IQueryable<Review>> GetDbSetReview();

        Task<List<ReviewDTO>?> GetVReviewsByConditions(GetReviewsRequest request);

        Task<PaginatedList<ReviewDTO>> GetReviewInformation(GetReviewsRequest request, Paging paging);

        Task<List<RatingDetails>> GetRatingDetails(int courseId);
    }
}
