using EduTrailblaze.Services.Models;
using Microsoft.Identity.Client;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> Register(RegisterModel registerModel);
        Task<ApiResponse> Login(LoginModel loginModel);
        Task<ApiResponse> VerifyAuthenticatorCode(string userId, string code);
        Task RemoveAuthenticator();
        Task<ApiResponse> ForgotPassword(ForgotPasswordModel forgotPasswordModel);
        Task<ApiResponse> ResetPassword(ResetPasswordModel resetPasswordModel);

        Task EnableAuthenticator(TwoFactorAuthenticationModel twoFactorAuthenticationViewModel);
        Task ExternalLoginCallback(AuthenticationResult authenticationResult);
        Task<ApiResponse> Logout(string userId);
        Task<ApiResponse> SignInWithGoogle(string redirectUrl);
        Task<ApiResponse> RefreshToken(string userId, string refreshToken);
    }
}
