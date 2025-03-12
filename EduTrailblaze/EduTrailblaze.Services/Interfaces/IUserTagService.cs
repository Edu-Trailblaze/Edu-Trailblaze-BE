using EduTrailblaze.Entities;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IUserTagService
    {
        Task<UserTag?> GetUserTag(int userTagId);
        Task<IEnumerable<UserTag>> GetUserTags();
        Task AddUserTag(UserTag userTag);
        Task UpdateUserTag(UserTag userTag);
        Task DeleteUserTag(int userTagId);
    }
}
