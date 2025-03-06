using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class UserCertificateService : IUserCertificateService
    {
        private readonly IRepository<UserCertificate, int> _userCertificateRepository;

        public UserCertificateService(IRepository<UserCertificate, int> userCertificateRepository)
        {
            _userCertificateRepository = userCertificateRepository;
        }

        public async Task<UserCertificate?> GetUserCertificate(int userCertificateId)
        {
            try
            {
                return await _userCertificateRepository.GetByIdAsync(userCertificateId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userCertificate: " + ex.Message);
            }
        }

        public async Task<IEnumerable<UserCertificate>> GetUserCertificates()
        {
            try
            {
                return await _userCertificateRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userCertificate: " + ex.Message);
            }
        }

        public async Task AddUserCertificate(UserCertificate userCertificate)
        {
            try
            {
                await _userCertificateRepository.AddAsync(userCertificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the userCertificate: " + ex.Message);
            }
        }

        public async Task UpdateUserCertificate(UserCertificate userCertificate)
        {
            try
            {
                await _userCertificateRepository.UpdateAsync(userCertificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the userCertificate: " + ex.Message);
            }
        }

        public async Task DeleteUserCertificate(UserCertificate userCertificate)
        {
            try
            {
                await _userCertificateRepository.DeleteAsync(userCertificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the userCertificate: " + ex.Message);
            }
        }
    }
}
