using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollment()
        {
            try
            {
                var news = await _enrollmentService.GetEnrollments();
                return Ok(news);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{enrollmentId}")]
        public async Task<IActionResult> GetEnrollment(int enrollmentId)
        {
            try
            {
                var news = await _enrollmentService.GetEnrollment(enrollmentId);

                if (news == null)
                {
                    return NotFound();
                }

                return Ok(news);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEnrollment([FromBody] CreateEnrollRequest enrollRequest)
        {
            try
            {
                await _enrollmentService.AddEnrollment(enrollRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-student-courses")]
        public async Task<IActionResult> GetStudentCourses([FromQuery] GetStudentCourses request)
        {
            try
            {
                var studentCourses = await _enrollmentService.GetStudentCourses(request);
                return Ok(studentCourses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
