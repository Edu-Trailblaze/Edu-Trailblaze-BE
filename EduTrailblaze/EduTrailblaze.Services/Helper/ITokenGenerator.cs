using EduTrailblaze.Entities;

namespace EduTrailblaze.Services.Helper
{
    public interface ITokenGenerator
    {
        Task<string> GenerateJwtToken(User user, string? name, string role);
        Task<string> GenerateRefreshToken();
    }
}
