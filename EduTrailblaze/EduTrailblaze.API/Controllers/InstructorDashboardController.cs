using EduTrailblaze.Services.Interfaces;
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

        [HttpGet("total-courses")]
        public async Task<IActionResult> GetTotalCourses(string instructorId, string time)
        {
            try
            {
                var data = await _instructorDashboardService.GetTotalCourses(instructorId, time);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("total-enrollments")]
        public async Task<IActionResult> GetTotalEnrollments(string instructorId, string time)
        {
            try
            {
                var data = await _instructorDashboardService.GetTotalEnrollments(instructorId, time);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("average-rating")]
        public async Task<IActionResult> GetAverageRating(string instructorId, string time)
        {
            try
            {
                var data = await _instructorDashboardService.GetAverageRating(instructorId, time);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue(string instructorId, string time)
        {
            try
            {
                var data = await _instructorDashboardService.GetTotalRevenue(instructorId, time);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
