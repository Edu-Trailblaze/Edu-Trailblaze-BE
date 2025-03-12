using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class UserTagService : IUserTagService
    {
        private readonly IRepository<UserTag, int> _userTagRepository;

        public UserTagService(IRepository<UserTag, int> userTagRepository)
        {
            _userTagRepository = userTagRepository;
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

        public async Task AddUserTag(UserTag userTag)
        {
            try
            {
                await _userTagRepository.AddAsync(userTag);
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