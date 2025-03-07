using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<Enrollment?> GetEnrollment(int enrollmentId);

        Task<IEnumerable<Enrollment>> GetEnrollments();

        Task EnrollCourse(CreateEnrollRequest enrollment);

        Task UpdateEnrollment(Enrollment enrollment);

        Task DeleteEnrollment(Enrollment enrollment);

        Task<int> GetNumberOfStudentsEnrolledInCourse(int courseId);

        Task<List<StudentCourseResponse>> GetStudentCourses(GetStudentCourses request);

        Task<int> GetStudentCourseClass(string userId, int courseId);

        Task<CourseStatus> CheckCourseStatus(string studentId, string courseId);
    }
}