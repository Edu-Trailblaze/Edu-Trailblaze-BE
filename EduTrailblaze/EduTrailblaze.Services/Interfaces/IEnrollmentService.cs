using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<Enrollment?> GetEnrollment(int enrollmentId);

        Task<IEnumerable<Enrollment>> GetEnrollments();
        Task<List<TopStudentResponse>> GetTop5StudentsWithMostEnrollments();
        Task EnrollCourse(CreateEnrollRequest enrollment);

        Task UpdateEnrollment(Enrollment enrollment);

        Task DeleteEnrollment(Enrollment enrollment);

        Task<int> GetNumberOfStudentsEnrolledInCourse(int courseId);

        Task<List<StudentCourseResponse>> GetStudentCourses(GetStudentCourses request);

        Task<int> GetStudentCourseClass(string userId, int courseId);

        Task<CourseStatus> CheckCourseStatus(string studentId, int courseId);

        Task<Enrollment> GetByCourseClassAndStudent(int courseClassId, string studentId);

        Task<StudentLearningCoursesResponse> GetStudentLearningCoursesRequest(GetStudentLearningCoursesRequest request);

        Task<StudentCourseProgressResponse> StudentCourseProgress(string userId, int courseId);
    }
}