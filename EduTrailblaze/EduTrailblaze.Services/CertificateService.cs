using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IRepository<Certificate, int> _certificateRepository;

        public CertificateService(IRepository<Certificate, int> certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Certificate?> GetCertificate(int certificateId)
        {
            try
            {
                return await _certificateRepository.GetByIdAsync(certificateId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the Certificate: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Certificate>> GetCertificates()
        {
            try
            {
                return await _certificateRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the Certificate: " + ex.Message);
            }
        }

        public async Task AddCertificate(Certificate certificate)
        {
            try
            {
                await _certificateRepository.AddAsync(certificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the Certificate: " + ex.Message);
            }
        }

        public async Task AddCertificate(CreateCertificateRequest certificate)
        {
            try
            {
                var newCertificate = new Certificate
                {
                    CourseId = certificate.CourseId,
                    CertificateTemplateUrl = certificate.CertificateTemplateUrl,
                };
                await _certificateRepository.AddAsync(newCertificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the Certificate: " + ex.Message);
            }
        }

        public async Task UpdateCertificate(Certificate certificate)
        {
            try
            {
                await _certificateRepository.UpdateAsync(certificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the Certificate: " + ex.Message);
            }
        }

        public async Task UpdateCertificate(UpdateCertificateRequest certificate)
        {
            try
            {
                var existingCertificate = await _certificateRepository.GetByIdAsync(certificate.CertificateId);
                if (existingCertificate == null)
                {
                    throw new Exception("Certificate not found.");
                }
                existingCertificate.CourseId = certificate.CourseId;
                existingCertificate.CertificateTemplateUrl = certificate.CertificateTemplateUrl;
                existingCertificate.UpdatedAt = DateTimeHelper.GetVietnamTime();

                await _certificateRepository.UpdateAsync(existingCertificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the Certificate: " + ex.Message);
            }
        }

        public async Task DeleteCertificate(Certificate certificate)
        {
            try
            {
                await _certificateRepository.DeleteAsync(certificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the Certificate: " + ex.Message);
            }
        }

        public async Task DeleteCertificate(int certificateId)
        {
            try
            {
                var existingCertificate = await _certificateRepository.GetByIdAsync(certificateId);
                if (existingCertificate == null)
                {
                    throw new Exception("Certificate not found.");
                }

                existingCertificate.IsDeleted = true;
                existingCertificate.UpdatedAt = DateTimeHelper.GetVietnamTime();

                await _certificateRepository.UpdateAsync(existingCertificate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the Certificate: " + ex.Message);
            }
        }
    }
}
