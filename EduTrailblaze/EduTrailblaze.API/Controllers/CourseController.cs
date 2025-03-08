using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                var courses = await _courseService.GetCourses();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            try
            {
                var course = await _courseService.GetCourse(courseId);
                if (course == null)
                {
                    return NotFound();
                }
                return Ok(course);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-instructors-of-a-course")]
        public async Task<IActionResult> GetInstructorInformationOfACourse(int courseId)
        {
            try
            {
                var course = await _courseService.InstructorInformation(courseId);
                if (course == null) return NotFound();
                return Ok(course);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse([FromForm] CreateCourseRequest course)
        {
            try
            {
                var result = await _courseService.AddCourse(course);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCourse([FromForm] UpdateCourseRequest course)
        {
            try
            {
                await _courseService.UpdateCourse(course);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            try
            {
                await _courseService.DeleteCourse(courseId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-courses-by-condition")]
        public async Task<IActionResult> GetCoursesByConditions([FromQuery] GetCoursesRequest request)
        {
            try
            {
                var reviews = await _courseService.GetCoursesByConditions(request);
                if (reviews == null) return NotFound();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-effective-price")]
        public async Task<IActionResult> GetEffectivePrice(int courseId)
        {
            try
            {
                var price = await _courseService.CalculateEffectivePrice(courseId);
                return Ok(price);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-course-information")]
        public async Task<IActionResult> GetCourseInformation([FromQuery] GetCoursesRequest request)
        {
            try
            {
                var res = await _courseService.GetCourseInformation(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-tag-information")]
        public async Task<IActionResult> GetTagInformation([FromQuery] int courseId)
        {
            try
            {
                var res = await _courseService.GetTagInformation(courseId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-paging-course-information")]
        public async Task<IActionResult> GetPagingCourseInformation([FromQuery] GetCoursesRequest request, [FromQuery] Paging paging)
        {
            try
            {
                var res = await _courseService.GetPagingCourseInformation(request, paging);
                if (res == null) return NotFound();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-course-detail/{courseId}")]
        public async Task<IActionResult> GetCourseSectionDetailsById(int courseId)
        {
            try
            {
                var res = await _courseService.GetCourseSectionDetailsById(courseId);
                if (res == null) return NotFound();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-course-page-information/{courseId}")]
        public async Task<IActionResult> GetCoursePageInformation(int courseId)
        {
            try
            {
                var res = await _courseService.GetCoursePageInformation(courseId);
                if (res == null) return NotFound();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-personal-item-recommendation")]
        public async Task<IActionResult> GetPersonalItemRecommendation(string? userId)
        {
            try
            {
                var res = await _courseService.GetPersonalItemRecommendation(userId);
                if (res == null) return NotFound();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
