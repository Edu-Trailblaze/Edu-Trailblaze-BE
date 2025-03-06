using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IRepository<Enrollment, int> _enrollmentRepository;

        public EnrollmentService(IRepository<Enrollment, int> enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<Enrollment?> GetEnrollment(int enrollmentId)
        {
            try
            {
                return await _enrollmentRepository.GetByIdAsync(enrollmentId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the enrollment.", ex);
            }
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollments()
        {
            try
            {
                return await _enrollmentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the enrollment.", ex);
            }
        }

        public async Task AddEnrollment(CreateEnrollRequest enrollment)
        {
            try
            {
                var isEnrollmentExists = await _enrollmentRepository.FindByCondition(e => e.StudentId == enrollment.StudentId && e.CourseClassId == enrollment.CourseClassId).AnyAsync();

                if (isEnrollmentExists)
                {
                    throw new Exception("User Is Already Enroll");
                }
                var newEnrollment = new Enrollment
                {
                    StudentId = enrollment.StudentId,
                    CourseClassId = enrollment.CourseClassId
                };

                await _enrollmentRepository.AddAsync(newEnrollment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the enrollment.", ex);
            }
        }

        public async Task UpdateEnrollment(Enrollment enrollment)
        {
            try
            {
                await _enrollmentRepository.UpdateAsync(enrollment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the enrollment.", ex);
            }
        }

        public async Task DeleteEnrollment(Enrollment enrollment)
        {
            try
            {
                await _enrollmentRepository.DeleteAsync(enrollment);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the enrollment.", ex);
            }
        }

        public async Task<int> GetNumberOfStudentsEnrolledInCourse(int courseId)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetAllAsync();
                return enrollments.Count(e => e.Id == courseId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the number of students enrolled in the course.", ex);
            }
        }
    }
}
