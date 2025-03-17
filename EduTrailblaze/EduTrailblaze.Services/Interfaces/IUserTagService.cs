using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IUserTagService
    {
        Task<UserTag?> GetUserTag(int userTagId);
        Task<IEnumerable<UserTag>> GetUserTags();
        Task<bool> AddOrUpdateUserTag(AddOrUpdateTagRequest request);
        Task UpdateUserTag(UserTag userTag);
        Task DeleteUserTag(int userTagId);
    }
}
