using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IUserCertificateService
    {
        Task<UserCertificate?> GetUserCertificate(int userCertificateId);

        Task<IEnumerable<UserCertificate>> GetUserCertificates();

        Task AddUserCertificate(UserCertificate userCertificate);

        Task UpdateUserCertificate(UserCertificate userCertificate);

        Task DeleteUserCertificate(UserCertificate userCertificate);

        Task AddUserCertificate(CreateUserCertificateRequest userCertificate);
    }
}
