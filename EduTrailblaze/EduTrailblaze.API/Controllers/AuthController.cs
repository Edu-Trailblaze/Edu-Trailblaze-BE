using EduTrailblaze.Entities;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Services.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenGenerator _jwtToken;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IRoleService _roleService;

        public AuthController(TokenGenerator tokenGenerator, UserManager<User> userManager, SignInManager<User> signInManager, IAuthService authService, IRoleService roleService)
        {
            _jwtToken = tokenGenerator;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _roleService = roleService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.Register(model);

            if (result.StatusCode == 200)
            {
                return Ok(new { Message = result });

            }
            return StatusCode(result.StatusCode, result);

        }

        [HttpPost("login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.Login(model);

            if (result.StatusCode == 200)
            {
                //if (result.Data is null)
                //{
                //    return File(QrcodeGenerator.GenerateQRCode(result.Message), "image/png");

                //}
                return Ok(new { Message = result });
            }
            return StatusCode(result.StatusCode, result);

        }

        [HttpPost("logout")]

        public async Task<IActionResult> Logout(string userId)
        {
            //  var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User not found" });
            }
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("signature");
            var result = await _authService.Logout(userId);

            if (result.StatusCode == 200)
            {
                return Ok(new { Message = result });
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {


            //var refreshToken = Request.Cookies["refreshToken"];
            //if (refreshToken == null)
            //{
            //    return BadRequest(new { Message = "Refresh token is required" });
            //}

            var result = await _authService.RefreshToken(model.UserId, model.Token);

            if (result.StatusCode == 200)
            {
                return Ok(new { Message = result });
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("VerifyAuthenticatorCode")]
        public async Task<IActionResult> EnableAuthenticator([FromBody] TwoFactorAuthenticationModel model)
        {
            var result = await _authService.VerifyAuthenticatorCode(model.UserId, model.Code);

            if (result.StatusCode == 200)
            {
                return Ok(new { Message = result });
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("forgot-Password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {

            var result = await _authService.ForgotPassword(model);

            if (result.StatusCode == 200)
            {
                return Ok(new { Message = result });
            }
            return StatusCode(result.StatusCode, result);

        }

        [HttpGet]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid password reset token or email.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
            if (!isTokenValid)
            {
                return BadRequest("Invalid token.");
            }

            // Redirect to the React app's reset password page with token and email
            var resetPasswordUrl = $"https://localhost:3000/reset-password?token={token}&email={email}";
            return Redirect(resetPasswordUrl);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {

            var result = await _authService.ResetPassword(model);

            if (result.StatusCode == 200)
            {
                return Ok(new { Message = result });
            }
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("change-role")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> ChangeRoleToInstructor([FromBody] ChangeInstructorRoleModel model)
        {
            var result = await _roleService.ChangeRoleToInstructor(model);

            if (result.StatusCode == 200)
            {
                return Ok(new { Message = result });
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("admin/assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            var result = await _roleService.AssignRole(model);

            if (result.StatusCode == 200)
            {
                return Ok(new { Message = result });
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("signin-facebook")]
        public IActionResult SignInWithFacebook()
        {
            try
            {
                var redirectUrl = Url.Action("FacebookResponse", "Authentication");
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(FacebookDefaults.AuthenticationScheme, redirectUrl);
                return Challenge(properties, FacebookDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("signin-google")]
        public async Task<IActionResult> SignInWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", null, Request.Scheme);
            var scheme = Request.Scheme ?? "https";
            var redirectUrl1 = $"{scheme}://{Request.Host}/authentication/google-response";


            var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);


            //if (redirectUrl == null)
            //{
            //    throw new ArgumentNullException(nameof(redirectUrl));
            //}

            //var result = await _authService.SignInWithGoogle(redirectUrl);

            //if (result.StatusCode == 200)
            //{
            //    if (result.Data == null)
            //    {
            //        return StatusCode(500, "Google authentication data is null.");
            //    }
            //    return Challenge(result.Data.ToString() ?? throw new ArgumentNullException(nameof(result.Data)), GoogleDefaults.AuthenticationScheme);
            //}
            //return StatusCode(result.StatusCode, result);
        }
        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (result == null || !result.Succeeded)
                {
                    return BadRequest("External authentication error");
                }

                var response = await _authService.HandleExternalLoginProviderCallBack(result);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}
