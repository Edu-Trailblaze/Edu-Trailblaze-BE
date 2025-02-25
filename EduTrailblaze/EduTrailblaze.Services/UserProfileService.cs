using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Firebase.Storage;
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

        public async Task UpdateUserProfile(string userId, UpdateUserProfileRequest userProfile)
        {
            try
            {
                var existingUserProfile = await _userProfileRepository.GetByIdAsync(userId);
                if (existingUserProfile == null)
                {
                    throw new Exception("UserProfile not found.");
                }

                existingUserProfile.Fullname = userProfile.FullName;
                if (userProfile.ProfilePicture != null)
                {
                    var file = userProfile.ProfilePicture;

                    if (file == null || file.Length == 0)
                    {
                        throw new Exception("File is empty.");
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    using (var stream = file.OpenReadStream())
                    {
                        var task = new FirebaseStorage("court-callers.appspot.com")
                            .Child("ProfileImage")
                            .Child(fileName)
                            .PutAsync(stream);

                        var downloadUrl = await task;
                        existingUserProfile.ProfilePictureUrl = downloadUrl;
                    }
                }
                else
                {
                    existingUserProfile.ProfilePictureUrl = "https://firebasestorage.googleapis.com/v0/b/storage-8b808.appspot.com/o/OIP.jpeg?alt=media&token=60195a0a-2fd6-4c66-9e3a-0f7f80eb8473";
                }

                await _userProfileRepository.UpdateAsync(existingUserProfile);
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
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
