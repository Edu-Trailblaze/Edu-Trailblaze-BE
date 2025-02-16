using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfiles()
        {
            try
            {
                var userProfile = await _userProfileService.GetUserProfiles();
                if (userProfile == null)
                {
                    return NotFound();
                }
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{userProfileId}")]
        public async Task<IActionResult> GetUserProfile(string userProfileId)
        {
            try
            {
                var userProfile = await _userProfileService.GetUserProfile(userProfileId);
                if (userProfile == null)
                {
                    return NotFound();
                }
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(string userId, [FromBody] UpdateUserProfileRequest userProfile)
        {
            try
            {
                await _userProfileService.UpdateUserProfile(userId, userProfile);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUserProfile([FromBody] CreateUserProfileRequest userProfile)
        {
            try
            {
                await _userProfileService.AddUserProfile(userProfile);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
