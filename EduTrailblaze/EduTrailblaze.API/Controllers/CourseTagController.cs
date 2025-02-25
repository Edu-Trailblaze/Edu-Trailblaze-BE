using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseTagController : ControllerBase
    {
        private readonly ICourseTagService _courseTagService;

        public CourseTagController(ICourseTagService courseTagService)
        {
            _courseTagService = courseTagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourseTags()
        {
            try
            {
                var courseTags = await _courseTagService.GetCourseTags();
                return Ok(courseTags);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseTag(int id)
        {
            try
            {
                var courseTag = await _courseTagService.GetCourseTag(id);
                return Ok(courseTag);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCourseTag([FromBody] CreateCourseTagRequest courseTag)
        {
            try
            {
                await _courseTagService.AddCourseTag(courseTag);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourseTag(int id)
        {
            try
            {
                var courseTag = await _courseTagService.GetCourseTag(id);
                if (courseTag == null)
                {
                    return NotFound();
                }
                await _courseTagService.DeleteCourseTag(courseTag);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
