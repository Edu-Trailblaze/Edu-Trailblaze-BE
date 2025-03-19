using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTagController : ControllerBase
    {
        private readonly IUserTagService _userTagService;

        public UserTagController(IUserTagService userTagService)
        {
            _userTagService = userTagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTags()
        {
            try
            {
                var userTags = await _userTagService.GetUserTags();
                return Ok(userTags);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{userTagId}")]
        public async Task<IActionResult> GetUserTag(int userTagId)
        {
            try
            {
                var userTag = await _userTagService.GetUserTag(userTagId);
                return Ok(userTag);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("AddOrUpdateTags")]
        public async Task<IActionResult> AddOrUpdateTags([FromBody] AddOrUpdateTagRequest request)
        {
            try
            {
                var result = await _userTagService.AddOrUpdateUserTag(request);

                if (result)
                {
                    return Ok(new { message = "Tags added or updated successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetUserTagByUserId/{userId}")]
        public async Task<IActionResult> GetUserTagByUserId(string userId)
        {
            try
            {
                var result = await _userTagService.GetUserTagByUserId(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
