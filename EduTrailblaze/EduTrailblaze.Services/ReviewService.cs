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

        //public async Task<List<ReviewDTO>?> GetVReviewsByConditions(GetReviewsRequest request)
        //{
        //    try
        //    {
        //        var dbSet = await _reviewRepository.GetDbSet();

        //        if (request.IsDeleted != null)
        //        {
        //            dbSet = dbSet.Where(c => c.IsDeleted == request.IsDeleted);
        //        }

        //        if (request.UserId != null)
        //        {
        //            dbSet = dbSet.Where(c => c.UserId == request.UserId);
        //        }

        //        if (request.CourseId != null)
        //        {
        //            dbSet = dbSet.Where(c => c.CourseId == request.CourseId);
        //        }

        //        if (request.ReviewText != null)
        //        {
        //            dbSet = dbSet.Where(c => c.ReviewText == request.ReviewText);
        //        }

        //        if (request.DiscountValueMax != null)
        //        {
        //            dbSet = dbSet.Where(c => c.DiscountValue <= request.DiscountValueMax);
        //        }

        //        if (request.StartDate != null)
        //        {
        //            dbSet = dbSet.Where(c => c.StartDate >= request.StartDate);
        //        }

        //        if (request.ExpiryDate != null)
        //        {
        //            dbSet = dbSet.Where(c => c.ExpiryDate <= request.ExpiryDate);
        //        }

        //        var items = await dbSet.ToListAsync();

        //        var voucherDTO = _mapper.Map<List<VoucherDTO>>(items);

        //        return voucherDTO;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while getting the courses: " + ex.Message);
        //    }
        //}

        //public async Task<PaginatedList<VoucherDTO>> GetVoucherInformation(GetVouchersRequest request, Paging paging)
        //{
        //    try
        //    {
        //        var vouchers = await GetVouchersByConditions(request);

        //        if (vouchers == null)
        //        {
        //            return new PaginatedList<VoucherDTO>(new List<VoucherDTO>(), 0, 1, 10);
        //        }

        //        if (!paging.PageSize.HasValue || paging.PageSize <= 0)
        //        {
        //            paging.PageSize = 10;
        //        }

        //        if (!paging.PageIndex.HasValue || paging.PageIndex <= 0)
        //        {
        //            paging.PageIndex = 1;
        //        }

        //        var totalCount = vouchers.Count;
        //        var skip = (paging.PageIndex.Value - 1) * paging.PageSize.Value;
        //        var take = paging.PageSize.Value;

        //        var validSortOptions = new[] { "highest_value", "order_value" };
        //        if (string.IsNullOrEmpty(paging.Sort) || !validSortOptions.Contains(paging.Sort))
        //        {
        //            paging.Sort = "highest_value";
        //        }

        //        vouchers = paging.Sort switch
        //        {
        //            "highest_value" => vouchers.OrderBy(p => p.DiscountType).ThenByDescending(p => p.DiscountValue).ToList(),
        //            "order_value" => vouchers.OrderByDescending(p => p.MinimumOrderValue).ToList(),
        //            _ => vouchers
        //        };

        //        var paginatedCourseCards = vouchers.Skip(skip).Take(take).ToList();

        //        return new PaginatedList<VoucherDTO>(paginatedCourseCards, totalCount, paging.PageIndex.Value, paging.PageSize.Value);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while getting the courses: " + ex.Message);
        //    }
        //}
    }
}
