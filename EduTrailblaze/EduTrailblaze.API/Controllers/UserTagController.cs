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
    }
}
