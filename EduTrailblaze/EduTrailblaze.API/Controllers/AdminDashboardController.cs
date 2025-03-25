using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        [HttpPut("approve-course")]
        public async Task<IActionResult> ApproveCourse(ApproveCourseRequest request)
        {
            try
            {
                await _adminDashboardService.ApproveCourse(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpPut("approve-course-by-ai")]
        //public async Task<IActionResult> ApproveCourseByAI(int courseId)
        //{
        //    try
        //    {
        //        await _adminDashboardService.ApproveCourseByAI(courseId);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        [HttpGet("number-of-instructors")]
        public async Task<IActionResult> NumberOfInstructors()
        {
            try
            {
                var numberOfInstructors = await _adminDashboardService.NumberOfInstructors();
                return Ok(numberOfInstructors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("number-of-students")]
        public async Task<IActionResult> NumberOfStudents()
        {
            try
            {
                var numberOfStudents = await _adminDashboardService.NumberOfStudents();
                return Ok(numberOfStudents);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> TotalRevenue()
        {
            try
            {
                var totalRevenue = await _adminDashboardService.TotalRevenue();
                return Ok(totalRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("total-courses-bought")]
        public async Task<IActionResult> TotalCoursesBought()
        {
            try
            {
                var totalCoursesBought = await _adminDashboardService.TotalCoursesBought();
                return Ok(totalCoursesBought);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("pending-courses")]
        public async Task<IActionResult> GetPendingCourses([FromQuery] Paging paging)
        {
            try
            {
                var pendingCourses = await _adminDashboardService.GetPendingCourses(paging);
                return Ok(pendingCourses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-course-data")]
        public async Task<IActionResult> GetCourseData([FromQuery] CourseDataRequest request)
        {
            try
            {
                var courseData = await _adminDashboardService.GetCourseData(request);
                return Ok(courseData);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
