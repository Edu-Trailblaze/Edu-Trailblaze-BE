using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfile?> GetUserProfile(string userProfileId);

        Task<IEnumerable<UserProfile>> GetUserProfiles();

        Task AddUserProfile(UserProfile userProfile);

        Task AddUserProfile(CreateUserProfileRequest userProfile);

        Task UpdateUserProfile(UserProfile userProfile);

        Task UpdateUserProfile(string userId, UpdateUserProfileRequest userProfile);

        Task DeleteUserProfile(UserProfile userProfile);
    }
}
