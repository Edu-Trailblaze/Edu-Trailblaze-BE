using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseClassController : ControllerBase
    {
        private readonly ICourseClassService _courseClassService;

        public CourseClassController(ICourseClassService courseClassService)
        {
            _courseClassService = courseClassService;
        }

        [HttpGet("{courseClassId}")]
        public async Task<IActionResult> GetCourseClass(int courseClassId)
        {
            try
            {
                var courseClass = await _courseClassService.GetCourseClass(courseClassId);
                if (courseClass == null)
                {
                    return NotFound();
                }
                return Ok(courseClass);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("newest/{courseId}")]
        public async Task<IActionResult> GetNewestCourseClass(int courseId)
        {
            try
            {
                var courseClass = await _courseClassService.GetNewestCourseClass(courseId);
                if (courseClass == null)
                {
                    return NotFound();
                }
                return Ok(courseClass);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseClasses()
        {
            try
            {
                var courseClasses = await _courseClassService.GetCourseClasses();
                return Ok(courseClasses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCourseClass([FromBody] CreateCourseClassRequest courseClass)
        {
            try
            {
                await _courseClassService.AddCourseClass(courseClass);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCourseClass([FromBody] UpdateCourseClassRequest courseClass)
        {
            try
            {
                await _courseClassService.UpdateCourseClass(courseClass);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
