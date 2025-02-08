using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ICourseInstructorService
    {
        Task<CourseInstructor?> GetCourseInstructor(int courseInstructorId);

        Task<IEnumerable<CourseInstructor>> GetCourseInstructors();

        Task AddCourseInstructor(CourseInstructor courseInstructor);

        Task UpdateCourseInstructor(CourseInstructor courseInstructor);

        Task DeleteCourseInstructor(CourseInstructor courseInstructor);

        Task RemoveAccessCourseInstructor(RemoveCourseInstructorRequest removeCourseInstructorRequest);

        Task AddCourseInstructor(CreateCourseInstructorRequest createCourseInstructorRequest);

        Task InviteCourseInstructor(InviteCourseInstructorRequest inviteCourseInstructorRequest);
    }
}
