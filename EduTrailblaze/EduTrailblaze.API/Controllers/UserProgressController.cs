using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProgressController : ControllerBase
    {
        private readonly IUserProgressService _userProgressService;

        public UserProgressController(IUserProgressService userProgressService)
        {
            _userProgressService = userProgressService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserProgress(SaveUserProgressRequest userProgressRequest)
        {
            try
            {
                await _userProgressService.SaveUserProgress(userProgressRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-user-progress")]
        public async Task<IActionResult> GetUserProgress(string userId, int? sectionId, int? lectureId, int? quizId)
        {
            try
            {
                var userProgress = await _userProgressService.GetUserProgress(userId, sectionId, lectureId, quizId);
                return Ok(userProgress);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
