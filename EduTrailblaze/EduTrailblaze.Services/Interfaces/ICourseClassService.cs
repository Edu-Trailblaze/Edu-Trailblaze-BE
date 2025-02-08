using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ICourseClassService
    {
        Task<CourseClass?> GetCourseClass(int courseClassId);

        Task<CourseClass?> GetNewestCourseClass(int courseId);

        Task<IEnumerable<CourseClass>> GetCourseClasses();

        Task AddCourseClass(CourseClass courseClass);

        Task UpdateCourseClass(CourseClass courseClass);

        Task AddCourseClass(CreateCourseClassRequest courseClass);

        Task UpdateCourseClass(UpdateCourseClassRequest courseClass);

        Task DeleteCourseClass(CourseClass courseClass);

        Task DeleteCourseClass(int courseClassId);
    }
}
