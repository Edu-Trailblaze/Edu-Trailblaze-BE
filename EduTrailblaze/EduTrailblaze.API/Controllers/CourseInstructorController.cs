using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseInstructorController : ControllerBase
    {
        private readonly ICourseInstructorService _courseInstructorService;

        public CourseInstructorController(ICourseInstructorService courseInstructorService)
        {
            _courseInstructorService = courseInstructorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseInstructors()
        {
            try
            {
                var courseInstructors = await _courseInstructorService.GetCourseInstructors();
                return Ok(courseInstructors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{courseInstructorId}")]
        public async Task<IActionResult> GetCourseInstructor(int courseInstructorId)
        {
            try
            {
                var courseInstructor = await _courseInstructorService.GetCourseInstructor(courseInstructorId);
                return Ok(courseInstructor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCourseInstructor([FromBody] CreateCourseInstructorRequest courseInstructor)
        {
            try
            {
                await _courseInstructorService.AddCourseInstructor(courseInstructor);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("invite-instructor")]
        public async Task<IActionResult> InviteInstructor([FromBody] InviteCourseInstructorRequest courseInstructor)
        {
            try
            {
                await _courseInstructorService.InviteCourseInstructor(courseInstructor);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCourseInstructorRequest([FromBody] RemoveCourseInstructorRequest courseInstructor)
        {
            try
            {
                await _courseInstructorService.RemoveAccessCourseInstructor(courseInstructor);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
