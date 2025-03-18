using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class UserTagService : IUserTagService
    {
        private readonly IRepository<UserTag, int> _userTagRepository;
        private readonly UserManager<User> _userManager;

        public UserTagService(IRepository<UserTag, int> userTagRepository, UserManager<User> userManager)
        {
            _userTagRepository = userTagRepository;
            _userManager = userManager;
        }

        public async Task<UserTag?> GetUserTag(int userTagId)
        {
            try
            {
                return await _userTagRepository.GetByIdAsync(userTagId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userTag: " + ex.Message);
            }
        }

        public async Task<IEnumerable<UserTag>> GetUserTags()
        {
            try
            {
                return await _userTagRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userTag: " + ex.Message);
            }
        }

        public async Task<bool> AddOrUpdateUserTag(AddOrUpdateTagRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);

                if (user == null)
                {
                    throw new ArgumentException("Invalid User ID");
                }

                var tagIds = request.TagId.ToList();
                var existingUserTags = await _userTagRepository
                    .FindByCondition(x => x.UserId == request.UserId, trackChanges: true)

                    .ToListAsync();
                var tagsToRemove = existingUserTags.Where(ut => !tagIds.Contains(ut.TagId)).ToList();
                await _userTagRepository.DeleteRangeAsync(tagsToRemove);
                foreach (var tagId in request.TagId)
                {
                    var existingTag = existingUserTags.FirstOrDefault(ut => ut.TagId == tagId);

                    if (existingTag != null)
                    {
                        await _userTagRepository.UpdateAsync(existingTag);
                    }
                    else
                    {
                        var newTag = new UserTag
                        {
                            UserId = request.UserId,
                            TagId = tagId

                        };
                        await _userTagRepository.AddAsync(newTag);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the userTag: " + ex.Message);
            }
        }

        public async Task UpdateUserTag(UserTag userTag)
        {
            try
            {
                var tag = await _userTagRepository.GetByIdAsync(userTag.Id);
                if (tag == null)
                {
                    throw new Exception("UserTag not found");
                }
                tag.TagId = userTag.TagId;
                tag.UserId = userTag.UserId;
                await _userTagRepository.UpdateAsync(tag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the userTag: " + ex.Message);
            }
        }

        public async Task DeleteUserTag(int userTagId)
        {
            try
            {
                var tag = await _userTagRepository.GetByIdAsync(userTagId);
                if (tag == null)
                {
                    throw new Exception("User not found");
                }
                await _userTagRepository.DeleteAsync(tag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the userTag: " + ex.Message);
            }
        }
    }
}