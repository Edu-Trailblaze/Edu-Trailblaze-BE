using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReview(int reviewId)
        {
            try
            {
                var review = await _reviewService.GetReview(reviewId);
                if (review == null) return NotFound();
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-paging-review")]
        public async Task<IActionResult> GetPagingReview([FromQuery] GetReviewsRequest request, [FromQuery] Paging paging)
        {
            try
            {
                var res = await _reviewService.GetReviewInformation(request, paging);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-review-by-condition")]
        public async Task<IActionResult> GetVReviewsByConditions([FromQuery] GetReviewsRequest request)
        {
            try
            {
                var res = await _reviewService.GetVReviewsByConditions(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-review-info/{courseId}")]
        public async Task<IActionResult> GetAverageRatingAndNumberOfRatings(int courseId)
        {
            try
            {
                var courseReviewResponse = await _reviewService.GetAverageRatingAndNumberOfRatings(courseId);
                return Ok(courseReviewResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewRequest review)
        {
            try
            {
                await _reviewService.AddReview(review);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewRequest review)
        {
            try
            {
                await _reviewService.UpdateReview(review);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                await _reviewService.DeleteReview(reviewId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
