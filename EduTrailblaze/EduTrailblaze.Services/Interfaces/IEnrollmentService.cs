using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<Enrollment?> GetEnrollment(int enrollmentId);

        Task<IEnumerable<Enrollment>> GetEnrollments();

        Task AddEnrollment(CreateEnrollRequest enrollment);

        Task UpdateEnrollment(Enrollment enrollment);

        Task DeleteEnrollment(Enrollment enrollment);

        Task<int> GetNumberOfStudentsEnrolledInCourse(int courseId);
    }
}