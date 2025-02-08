using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review, int> _reviewRepository;

        public ReviewService(IRepository<Review, int> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<IQueryable<Review>> GetDbSetReview()
        {
            try
            {
                return await _reviewRepository.GetDbSet();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the review.", ex);
            }
        }

        public async Task<Review?> GetReview(int reviewId)
        {
            try
            {
                return await _reviewRepository.GetByIdAsync(reviewId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the review.", ex);
            }
        }

        public async Task<IEnumerable<Review>> GetReviews()
        {
            try
            {
                return await _reviewRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the review.", ex);
            }
        }

        public async Task AddReview(Review review)
        {
            try
            {
                await _reviewRepository.AddAsync(review);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the review.", ex);
            }
        }

        public async Task UpdateReview(Review review)
        {
            try
            {
                await _reviewRepository.UpdateAsync(review);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the review.", ex);
            }
        }

        public async Task DeleteReview(Review review)
        {
            try
            {
                await _reviewRepository.DeleteAsync(review);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the review.", ex);
            }
        }

        public async Task AddReview(CreateReviewRequest review)
        {
            try
            {
                var newReview = new Review
                {
                    CourseId = review.CourseId,
                    UserId = review.UserId,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText
                };
                await _reviewRepository.AddAsync(newReview);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the review.", ex);
            }
        }

        public async Task UpdateReview(UpdateReviewRequest review)
        {
            try
            {
                var existingReview = await _reviewRepository.GetByIdAsync(review.ReviewId);
                if (existingReview == null)
                {
                    throw new Exception("Review not found.");
                }
                existingReview.Rating = review.Rating;
                existingReview.ReviewText = review.ReviewText;
                existingReview.UpdatedAt = DateTimeHelper.GetVietnamTime();
                await _reviewRepository.UpdateAsync(existingReview);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the review.", ex);
            }
        }

        public async Task DeleteReview(int reviewId)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId);
                if (review == null)
                {
                    throw new Exception("Review not found.");
                }
                review.IsDeleted = true;

                await _reviewRepository.UpdateAsync(review);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the review.", ex);
            }
        }

        public async Task<ReviewInformation> GetAverageRatingAndNumberOfRatings(int courseId)
        {
            try
            {
                var reviews = await _reviewRepository.GetAllAsync();
                var courseReviews = reviews.Where(r => r.CourseId == courseId);

                if (courseReviews.Count() == 0)
                {
                    return new ReviewInformation
                    {
                        AverageRating = 0,
                        TotalRatings = 0
                    };
                }

                var averageRating = courseReviews.Average(r => r.Rating);
                var numberOfRatings = courseReviews.Count();
                return new ReviewInformation
                {
                    AverageRating = averageRating,
                    TotalRatings = numberOfRatings
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the average rating: " + ex.Message);
            }
        }
    }
}
