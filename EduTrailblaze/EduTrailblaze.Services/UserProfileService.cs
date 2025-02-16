using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EduTrailblaze.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IRepository<UserProfile, string> _userProfileRepository;
        private readonly UserManager<User> _userManager;

        public UserProfileService(IRepository<UserProfile, string> userProfileRepository, UserManager<User> userManager)
        {
            _userProfileRepository = userProfileRepository;
            _userManager = userManager;
        }

        public async Task<UserProfile?> GetUserProfile(string userProfileId)
        {
            try
            {
                return await _userProfileRepository.GetByIdAsync(userProfileId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userProfile.", ex);
            }
        }

        public async Task<IEnumerable<UserProfile>> GetUserProfiles()
        {
            try
            {
                return await _userProfileRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userProfile.", ex);
            }
        }

        public async Task AddUserProfile(UserProfile userProfile)
        {
            try
            {
                await _userProfileRepository.AddAsync(userProfile);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the userProfile.", ex);
            }
        }

        public async Task UpdateUserProfile(UserProfile userProfile)
        {
            try
            {
                await _userProfileRepository.UpdateAsync(userProfile);
                var user = await _userManager.FindByIdAsync(userProfile.Id);
                if (user != null)
                {
                    user.UserProfile = userProfile;
                    await _userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the userProfile.", ex);
            }
        }

        public async Task AddUserProfile(CreateUserProfileRequest userProfile)
        {
            try
            {
                var newUserProfile = new UserProfile
                {
                    Id = userProfile.UserId,
                    Fullname = userProfile.FullName,
                };
                await _userProfileRepository.AddAsync(newUserProfile);
                
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the userProfile.", ex);
            }
        }

        public async Task UpdateUserProfile(UpdateUserProfileRequest userProfile)
        {
            try
            {
                var existingUserProfile = await _userProfileRepository.GetByIdAsync(userProfile.UserId);
                if (existingUserProfile == null)
                {
                    throw new Exception("UserProfile not found.");
                }
                existingUserProfile.Fullname = userProfile.FullName;
                existingUserProfile.ProfilePictureUrl = userProfile.ProfilePictureUrl;
                await _userProfileRepository.UpdateAsync(existingUserProfile);
                var user = await _userManager.FindByIdAsync(userProfile.UserId);
                if (user != null)
                {
                    user.UserName = userProfile.UserName;
                    user.Email = userProfile.Email;
                    user.PhoneNumber = userProfile.PhoneNumber;
                    await _userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the userProfile.", ex);
            }
        }

        public async Task DeleteUserProfile(UserProfile userProfile)
        {
            try
            {
                await _userProfileRepository.DeleteAsync(userProfile);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the userProfile.", ex);
            }
        }
    }
}
