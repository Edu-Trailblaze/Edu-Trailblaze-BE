using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser(string userId);

        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUserByEmail(string email);

        Task<List<UserDTO>?> GetUsersByConditions(GetUsersRequest request);

        Task<PaginatedList<UserDTO>> GetUserInformation(GetUsersRequest request, Paging paging);
    }
}
