using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class CourseLanguageService : ICourseLanguageService
    {
        private readonly IRepository<CourseLanguage, int> _courseLanguageRepository;

        public CourseLanguageService(IRepository<CourseLanguage, int> courseLanguageRepository)
        {
            _courseLanguageRepository = courseLanguageRepository;
        }

        public async Task<CourseLanguage?> GetCourseLanguage(int courseLanguageId)
        {
            try
            {
                return await _courseLanguageRepository.GetByIdAsync(courseLanguageId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseLanguage: " + ex.Message);
            }
        }

        public async Task<IEnumerable<CourseLanguage>> GetCourseLanguages()
        {
            try
            {
                return await _courseLanguageRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseLanguage: " + ex.Message);
            }
        }

        public async Task AddCourseLanguage(CourseLanguage courseLanguage)
        {
            try
            {
                await _courseLanguageRepository.AddAsync(courseLanguage);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseLanguage: " + ex.Message);
            }
        }

        public async Task UpdateCourseLanguage(CourseLanguage courseLanguage)
        {
            try
            {
                await _courseLanguageRepository.UpdateAsync(courseLanguage);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the courseLanguage: " + ex.Message);
            }
        }

        public async Task DeleteCourseLanguage(CourseLanguage courseLanguage)
        {
            try
            {
                await _courseLanguageRepository.DeleteAsync(courseLanguage);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the courseLanguage: " + ex.Message);
            }
        }
    }
}
