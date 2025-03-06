using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("{newsId}")]
        public async Task<IActionResult> GetNews(int newsId)
        {
            try
            {
                var news = await _enrollmentService.GetEnrollment(newsId);

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
        public async Task<IActionResult> AddNews([FromBody] CreateEnrollRequest enrollRequest)
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

        //[HttpPut]
        //public async Task<IActionResult> UpdateNews([FromBody] UpdateNewsRequest news)
        //{
        //    try
        //    {
        //        await _enrollmentService.UpdateEnrollment(news);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        //[HttpDelete("{newsId}")]
        //public async Task<IActionResult> DeleteNews(int newsId)
        //{
        //    try
        //    {
        //        await _enrollmentService.DeleteEnrollment(newsId);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}
    }
}
