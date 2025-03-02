using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Services.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorDashboardController : ControllerBase
    {
        private readonly IInstructorDashboardService _instructorDashboardService;

        public InstructorDashboardController(IInstructorDashboardService instructorDashboardService)
        {
            _instructorDashboardService = instructorDashboardService;
        }

        [HttpGet("get-total-courses")]
        public async Task<IActionResult> GetTotalCourses([FromQuery] InstructorDashboardRequest request)
        {
            try
            {
                var result = await _instructorDashboardService.GetTotalCourses(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-total-enrollments")]
        public async Task<IActionResult> GetTotalEnrollments([FromQuery] InstructorDashboardRequest request)
        {
            try
            {
                var result = await _instructorDashboardService.GetTotalEnrollments(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-avarage-rating")]
        public async Task<IActionResult> GetAverageRating([FromQuery] InstructorDashboardRequest request)
        {
            try
            {
                var result = await _instructorDashboardService.GetAverageRating(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-total-revenue")]
        public async Task<IActionResult> GetTotalRevenue([FromQuery] InstructorDashboardRequest request)
        {
            try
            {
                var result = await _instructorDashboardService.GetTotalRevenue(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-nearest-time-for-revenue")]
        public async Task<IActionResult> GetNearestTimeForRevenue([FromQuery] InstructorDashboardRequest request)
        {
            try
            {
                var result = await _instructorDashboardService.GetNearestTimeForRevenue(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-nearest-time-for-enrollments")]
        public async Task<IActionResult> GetNearestTimeForEnrollments([FromQuery] InstructorDashboardRequest request)
        {
            try
            {
                var result = await _instructorDashboardService.GetNearestTimeForEnrollments(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-course-completion-rate")]
        public async Task<IActionResult> GetCourseCompletionRate(string instructorId)
        {
            try
            {
                var result = await _instructorDashboardService.GetCourseCompletionRate(instructorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-top-performing-courses")]
        public async Task<IActionResult> GetTopPerformingCourses(string instructorId, int top)
        {
            try
            {
                var result = await _instructorDashboardService.GetTopPerformingCourses(instructorId, top);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
