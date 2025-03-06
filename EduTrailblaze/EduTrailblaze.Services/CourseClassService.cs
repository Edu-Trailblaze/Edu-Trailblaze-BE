using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class CourseClassService : ICourseClassService
    {
        private readonly IRepository<CourseClass, int> _courseClassRepository;

        public CourseClassService(IRepository<CourseClass, int> courseClassRepository)
        {
            _courseClassRepository = courseClassRepository;
        }

        public async Task<CourseClass?> GetCourseClass(int courseClassId)
        {
            try
            {
                return await _courseClassRepository.GetByIdAsync(courseClassId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the CourseClass.", ex);
            }
        }

        public async Task<IEnumerable<CourseClass>> GetCourseClasses()
        {
            try
            {
                return await _courseClassRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the CourseClass.", ex);
            }
        }

        public async Task AddCourseClass(CourseClass courseClass)
        {
            try
            {
                await _courseClassRepository.AddAsync(courseClass);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the CourseClass.", ex);
            }
        }

        public async Task UpdateCourseClass(CourseClass courseClass)
        {
            try
            {
                await _courseClassRepository.UpdateAsync(courseClass);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the CourseClass.", ex);
            }
        }

        public async Task AddCourseClass(CreateCourseClassRequest courseClass)
        {
            try
            {
                var newCourseClass = new CourseClass
                {
                    CourseId = courseClass.CourseId,
                    Title = courseClass.Title,
                    ImageURL = courseClass.ImageURL,
                    IntroURL = courseClass.IntroURL,
                    Description = courseClass.Description,
                    Price = courseClass.Price,
                    Duration = courseClass.Duration,
                    DifficultyLevel = courseClass.DifficultyLevel,
                    Prerequisites = courseClass.Prerequisites,
                    LearningOutcomes = courseClass.LearningOutcomes,
                    EstimatedCompletionTime = courseClass.EstimatedCompletionTime,
                    StartDate = courseClass.StartDate,
                    EndDate = courseClass.EndDate
                };
                await _courseClassRepository.AddAsync(newCourseClass);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the CourseClass: " + ex.Message);
            }
        }

        public async Task UpdateCourseClass(UpdateCourseClassRequest courseClass)
        {
            try
            {
                var existingCourseClass = await _courseClassRepository.GetByIdAsync(courseClass.Id);
                if (existingCourseClass == null)
                {
                    throw new Exception("CourseClass not found.");
                }
                existingCourseClass.CourseId = courseClass.CourseId;
                existingCourseClass.Title = courseClass.Title;
                existingCourseClass.ImageURL = courseClass.ImageURL;
                existingCourseClass.IntroURL = courseClass.IntroURL;
                existingCourseClass.Description = courseClass.Description;
                existingCourseClass.Price = courseClass.Price;
                existingCourseClass.Duration = courseClass.Duration;
                existingCourseClass.DifficultyLevel = courseClass.DifficultyLevel;
                existingCourseClass.Prerequisites = courseClass.Prerequisites;
                existingCourseClass.LearningOutcomes = courseClass.LearningOutcomes;
                existingCourseClass.EstimatedCompletionTime = courseClass.EstimatedCompletionTime;
                existingCourseClass.StartDate = courseClass.StartDate;
                existingCourseClass.EndDate = courseClass.EndDate;
                existingCourseClass.UpdatedAt = DateTimeHelper.GetVietnamTime();

                await _courseClassRepository.UpdateAsync(existingCourseClass);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the CourseClass.", ex);
            }
        }

        public async Task DeleteCourseClass(CourseClass courseClass)
        {
            try
            {
                await _courseClassRepository.DeleteAsync(courseClass);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the CourseClass.", ex);
            }
        }

        public async Task DeleteCourseClass(int courseClassId)
        {
            try
            {
                var courseClass = await _courseClassRepository.GetByIdAsync(courseClassId);
                if (courseClass == null)
                {
                    throw new Exception("CourseClass not found.");
                }
                await _courseClassRepository.DeleteAsync(courseClass);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the CourseClass.", ex);
            }
        }

        public async Task<CourseClass?> GetNewestCourseClass(int courseId)
        {
            try
            {
                return await (await _courseClassRepository.GetDbSet()).Where(x => x.CourseId == courseId).OrderByDescending(x => x.StartDate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the newest CourseClass.", ex);
            }
        }
    }
}
