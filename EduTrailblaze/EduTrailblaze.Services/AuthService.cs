using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace EduTrailblaze.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRedisService _redisService;
        private readonly ITokenGenerator _jwtToken;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly ISendMail _sendMail;
        private readonly IUserProfileService _userProfileService;

        public AuthService(ITokenGenerator authService, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IRedisService redisService, ISendMail sendMail, IUserProfileService userProfileService)
        {
            _jwtToken = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbRetryPolicy = Policy.Handle<SqlException>()
                                   .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                                   (exception, timeSpan, retryCount, context) =>
                                   {
                                       Console.WriteLine($"[Db Retry] Attempt {retryCount} after {timeSpan} due to: {exception.Message}");
                                   });
            _dbTimeoutPolicy = Policy.TimeoutAsync(10, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
            {
                Console.WriteLine($"[Db Timeout] Operation timed out after {timeSpan}");
                return Task.CompletedTask;
            });
            _dbPolicyWrap = Policy.WrapAsync(_dbRetryPolicy, _dbTimeoutPolicy);
            _redisService = redisService;
            _sendMail = sendMail;
            _userProfileService = userProfileService;
        }



        public Task EnableAuthenticator(TwoFactorAuthenticationModel twoFactorAuthenticationViewModel)
        {
            throw new NotImplementedException();
        }

        public Task ExternalLoginCallback()
        {
            throw new NotImplementedException();
        }

        public Task ExternalLoginCallback(AuthenticationResult authenticationResult)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(forgotPasswordModel.Email));
            if (user == null)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status404NotFound, Message = "User not found." };
            }
            var token = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.GeneratePasswordResetTokenAsync(user));

            var resetPasswordUrl = $"https://localhost:7034/reset-password?email={user.Email}&token={token}";

            var isSendMailSuccess = await _sendMail.SendForgotEmailAsync(forgotPasswordModel.Email, "Reset Password", resetPasswordUrl);
            return (isSendMailSuccess is true) ? new ApiResponse { StatusCode = StatusCodes.Status200OK, Message = "Email sent successfully." } : new ApiResponse { StatusCode = StatusCodes.Status500InternalServerError, Message = "Error sending email." };

        }

        public async Task<ApiResponse> Login(LoginModel loginModel)
        {
            try
            {
                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(loginModel.Email));

                if (user == null)
                {
                    return new ApiResponse { StatusCode = StatusCodes.Status400BadRequest, Data = "Does not have that account in the Application" };
                }

                var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut) return new ApiResponse { StatusCode = StatusCodes.Status401Unauthorized, Message = "Your account is locked. Please contact support." };
                    if (result.IsNotAllowed) return new ApiResponse { StatusCode = StatusCodes.Status401Unauthorized, Message = "Your account is not allowed to login. Please contact support." };
                    if (result.RequiresTwoFactor)
                        return new ApiResponse
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Message = await _userManager.GetAuthenticatorKeyAsync(user)
                        };
                    return new ApiResponse { StatusCode = StatusCodes.Status401Unauthorized, Data = "Invalid login attempt." };
                }
                var roles = await _userManager.GetRolesAsync(user);
                //if (await _userManager.GetTwoFactorEnabledAsync(user) is true) return new ApiResponse { StatusCode = StatusCodes.Status200OK, Data = new { QrCode = await _userManager.GetAuthenticatorKeyAsync(user) } };
                var claims = _userManager.GetClaimsAsync(user);
                var token = _jwtToken.GenerateJwtToken(user, roles[0].ToString());
                await Task.WhenAll(claims, token);
                var claimsasync = await claims;
                var tokenasync = await token;


                var firstNameClaim = claimsasync.FirstOrDefault(u => u.Type == "FirstName");
                if (firstNameClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, firstNameClaim);
                }
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Username", user.UserName));


                var refreshToken = await _jwtToken.GenerateRefreshToken();
                await _redisService.AcquireLock(user.Id, refreshToken);

                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Login successful.",
                    Data = new
                    {
                        AccessToken = tokenasync,
                        RefreshToken = refreshToken
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Error during registration: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> Logout(string userId)
        {
            try
            {
                var isReleaseLockSuccess = await _redisService.ReleaseLock(userId);
                if (!isReleaseLockSuccess)
                {
                    return new ApiResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "User not found"
                    };
                }

                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Logged out successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Error during logout: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse> RefreshToken(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status404NotFound, Message = "User not found." };
            }

            var isValidRefreshToken = await _redisService.CheckRefreshToken(userId, refreshToken);
            if (!isValidRefreshToken)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status401Unauthorized, Message = "Invalid refresh token." };
            }

            var token = _jwtToken.GenerateJwtToken(user, "Student");
            var newRefreshToken = _jwtToken.GenerateRefreshToken();
            Task.WhenAll(token, newRefreshToken);
            var tokenAsync = await token;
            var newRefreshTokenAsync = await newRefreshToken;
            return new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Token refreshed successfully.",
                Data = new
                {
                    AccessToken = tokenAsync,
                    RefreshToken = newRefreshTokenAsync
                }
            };
        }

        public async Task<ApiResponse> Register(RegisterModel model)
        {
            try
            {
                if (model == null)
                {
                    return new ApiResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid request."
                    };
                }

                if (model.Password != model.ConfirmPassword)
                {
                    return new ApiResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Passwords do not match."
                    };
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && user.EmailConfirmed)
                {
                    return new ApiResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "User already exists and is confirmed"
                    };
                }

                var newUser = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(newUser, model.Password);

                if (!result.Succeeded)
                {
                    return new ApiResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = $"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                    };
                }
                await _userManager.AddToRoleAsync(newUser, "Student");

                await _userManager.SetTwoFactorEnabledAsync(newUser, false);
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                var userLogin = await _userManager.FindByEmailAsync(model.Email);

                await _userManager.ResetAuthenticatorKeyAsync(newUser);


                var token = await _jwtToken.GenerateJwtToken(userLogin, "Student");
                var refreshToken = await _jwtToken.GenerateRefreshToken();


                CreateUserProfileRequest userProfile =
                    new CreateUserProfileRequest
                    {
                        UserId = userLogin.Id
                    };
                await _userProfileService.AddUserProfile(userProfile);

                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User registered successfully.",
                    Data = new
                    {
                        AccessToken = token,
                        RefreshToken = refreshToken
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Error during registration: {ex.Message}"
                };
            }
        }

        public Task RemoveAuthenticator()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(resetPasswordModel.Email));
            if (user == null)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status404NotFound, Message = "User not found." };
            }
            var result = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password));
            return (result.Succeeded) ? new ApiResponse { StatusCode = StatusCodes.Status200OK, Message = "Password reset successfully." } : new ApiResponse { StatusCode = StatusCodes.Status500InternalServerError, Message = "Error when reset password." };
        }

        public async Task<ApiResponse> VerifyAuthenticatorCode(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found."
                };
            }
            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Two-factor authentication is not enabled for this user."
                };
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);

            if (!isValid)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status401Unauthorized, Message = "Invalid 2FA code." };
            }

            var token = await _jwtToken.GenerateJwtToken(user, "Admin");
            var refreshToken = await _jwtToken.GenerateRefreshToken();
            await _redisService.AcquireLock(user.Id, refreshToken);

            return new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Login successful.",
                Data = new
                {
                    AccessToken = token,
                    RefreshToken = refreshToken
                }
            };
        }
    }
}