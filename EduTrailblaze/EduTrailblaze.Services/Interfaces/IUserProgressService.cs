using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IUserProgressService
    {
        Task<UserProgress?> GetUserProgress(int userProgressId);

        Task<IEnumerable<UserProgress>> GetUserProgresss();

        Task AddUserProgress(UserProgress userProgress);

        Task UpdateUserProgress(UserProgress userProgress);

        Task DeleteUserProgress(UserProgress userProgress);

        Task SaveUserProgress(SaveUserProgressRequest userProgressRequest);
    }
}
