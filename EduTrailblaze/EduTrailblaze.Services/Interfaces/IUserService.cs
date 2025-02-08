using EduTrailblaze.Entities;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser(string userId);

        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUserByEmail(string email);
    }
}
