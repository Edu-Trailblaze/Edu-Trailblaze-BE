using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ICourseTagService
    {
        Task<CourseTag?> GetCourseTag(int courseTagId);

        Task<IEnumerable<CourseTag>> GetCourseTags();

        Task AddCourseTag(CourseTag courseTag);

        Task UpdateCourseTag(CourseTag courseTag);

        Task DeleteCourseTag(CourseTag courseTag);

        Task AddCourseTag(CreateCourseTagRequest courseTag);
    }
}
