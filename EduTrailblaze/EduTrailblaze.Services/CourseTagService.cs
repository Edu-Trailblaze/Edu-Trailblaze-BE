using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class CourseTagService : ICourseTagService
    {
        private readonly IRepository<CourseTag, int> _courseTagRepository;

        public CourseTagService(IRepository<CourseTag, int> courseTagRepository)
        {
            _courseTagRepository = courseTagRepository;
        }

        public async Task<CourseTag?> GetCourseTag(int courseTagId)
        {
            try
            {
                return await _courseTagRepository.GetByIdAsync(courseTagId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseTag.", ex);
            }
        }

        public async Task<IEnumerable<CourseTag>> GetCourseTags()
        {
            try
            {
                return await _courseTagRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseTag.", ex);
            }
        }

        public async Task AddCourseTag(CourseTag courseTag)
        {
            try
            {
                await _courseTagRepository.AddAsync(courseTag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseTag.", ex);
            }
        }

        public async Task AddCourseTag(CreateCourseTagRequest courseTag)
        {
            try
            {
                var newCourseTag = new CourseTag
                {
                    CourseId = courseTag.CourseId,
                    TagId = courseTag.TagId
                };
                await _courseTagRepository.AddAsync(newCourseTag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseTag.", ex);
            }
        }

        public async Task UpdateCourseTag(CourseTag courseTag)
        {
            try
            {
                await _courseTagRepository.UpdateAsync(courseTag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the courseTag.", ex);
            }
        }

        public async Task DeleteCourseTag(CourseTag courseTag)
        {
            try
            {
                await _courseTagRepository.DeleteAsync(courseTag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the courseTag.", ex);
            }
        }
    }
}
