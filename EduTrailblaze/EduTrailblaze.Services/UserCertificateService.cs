using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using Firebase.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class UserCertificateService : IUserCertificateService
    {
        private readonly IRepository<UserCertificate, int> _userCertificateRepository;
        private readonly IRepository<Certificate, int> _certificateRepository;
        private readonly ICourseService _courseService;
        private readonly UserManager<User> _userManager;

        public UserCertificateService(IRepository<UserCertificate, int> userCertificateRepository, ICourseService courseService, IRepository<Certificate, int> certificateRepository, UserManager<User> userManager)
        {
            _userCertificateRepository = userCertificateRepository;
            _courseService = courseService;
            _certificateRepository = certificateRepository;
            _userManager = userManager;
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

        public async Task AddUserCertificate(CreateUserCertificateRequest userCertificate)
        {
            try
            {
                var course = await _courseService.GetCourse(userCertificate.CourseId);
                if (course == null)
                {
                    throw new Exception("Course not found");
                }
                var certificateDbSet = await _certificateRepository.GetDbSet();
                var certificate = certificateDbSet.FirstOrDefault(c => c.CourseId == userCertificate.CourseId);

                if (certificate == null)
                {
                    throw new Exception("Certificate not found");
                }

                var user = await _userManager.FindByIdAsync(userCertificate.UserId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Certificate.html");
                var templateContent = await File.ReadAllTextAsync(templatePath);

                var filledTemplate = templateContent
                    .Replace("[Student's Full Name]", user.Email)
                    .Replace("[Course Title]", course.Title)
                    //.Replace("[Start Date]", course.StartDate.ToString("MMMM dd, yyyy"))
                    //.Replace("[End Date]", course.EndDate.ToString("MMMM dd, yyyy"))
                    //.Replace("[Number of Hours]", course.Duration.ToString())
                    //.Replace("[ABC123XYZ]", Guid.NewGuid().ToString())
                    .Replace("[Date]", DateTimeHelper.GetVietnamTime().ToString("MMMM dd, yyyy"));

                var imageBytes = await FileConverter.ConvertHtmlToImage(filledTemplate);

                var fileName = Guid.NewGuid() + ".png";
                string certificateUrl = "";
                using (var stream = new MemoryStream(imageBytes))
                {
                    var task = new FirebaseStorage("court-callers.appspot.com")
                        .Child("Certificates")
                        .Child(fileName)
                        .PutAsync(stream);

                    var downloadUrl = await task;
                    certificateUrl = downloadUrl;
                }

                var userCertificateEntity = new UserCertificate
                {
                    CertificateId = certificate.CourseId,
                    UserId = userCertificate.UserId,
                    CertificateUrl = certificateUrl,
                    IssuedAt = DateTimeHelper.GetVietnamTime(),
                };
                await _userCertificateRepository.AddAsync(userCertificateEntity);
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

        public async Task<List<CourseCertificatesResponse>> GetUserCertificatesByConditions(GetCourseCertificatesRequest request)
        {
            try
            {
                var userCertificatesQuery = await _userCertificateRepository.GetDbSet();

                if (request.CourseId.HasValue)
                {
                    userCertificatesQuery = userCertificatesQuery.Where(uc => uc.Certificate.CourseId == request.CourseId.Value);
                }

                if (request.UserId != null)
                {
                    userCertificatesQuery = userCertificatesQuery.Where(uc => uc.UserId == request.UserId);
                }

                if (request.FromDate.HasValue)
                {
                    userCertificatesQuery = userCertificatesQuery.Where(uc => uc.IssuedAt >= request.FromDate.Value);
                }

                if (request.ToDate.HasValue)
                {
                    userCertificatesQuery = userCertificatesQuery.Where(uc => uc.IssuedAt <= request.ToDate.Value);
                }

                var userCertificates = await userCertificatesQuery.ToListAsync();
                var certificateResponses = userCertificates.Select(uc => new CourseCertificatesResponse
                {
                    CertificateUrl = uc.CertificateUrl,
                    UserId = uc.UserId,
                    CourseId = uc.Certificate.CourseId,
                    IssuedAt = uc.IssuedAt
                }).ToList();

                return certificateResponses;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userCertificates: " + ex.Message);
            }
        }
    }
}
