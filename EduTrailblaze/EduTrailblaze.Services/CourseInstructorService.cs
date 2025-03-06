using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class CourseInstructorService : ICourseInstructorService
    {
        private readonly IRepository<CourseInstructor, int> _courseInstructorRepository;

        public CourseInstructorService(IRepository<CourseInstructor, int> courseInstructorRepository)
        {
            _courseInstructorRepository = courseInstructorRepository;
        }

        public async Task<CourseInstructor?> GetCourseInstructor(int courseInstructorId)
        {
            try
            {
                return await _courseInstructorRepository.GetByIdAsync(courseInstructorId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseInstructor: " + ex.Message);
            }
        }

        public async Task<CourseInstructor?> GetCourseInstructor(int courseId, string instructorId)
        {
            try
            {
                var dbSet = await _courseInstructorRepository.GetDbSet();
                var courseInstructor = await dbSet.FirstOrDefaultAsync(x => x.CourseId == courseId && x.InstructorId == instructorId);
                return courseInstructor;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseInstructor: " + ex.Message);
            }
        }

        public async Task<IEnumerable<CourseInstructor>> GetCourseInstructors()
        {
            try
            {
                return await _courseInstructorRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the courseInstructor: " + ex.Message);
            }
        }

        public async Task AddCourseInstructor(CourseInstructor courseInstructor)
        {
            try
            {
                await _courseInstructorRepository.AddAsync(courseInstructor);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseInstructor: " + ex.Message);
            }
        }

        public async Task AddCourseInstructor(CreateCourseInstructorRequest createCourseInstructorRequest)
        {
            try
            {
                var courseInstructor = new CourseInstructor
                {
                    CourseId = createCourseInstructorRequest.CourseId,
                    InstructorId = createCourseInstructorRequest.InstructorId,
                    IsPrimaryInstructor = createCourseInstructorRequest.IsPrimaryInstructor
                };
                await _courseInstructorRepository.AddAsync(courseInstructor);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseInstructor: " + ex.Message);
            }
        }

        public async Task InviteCourseInstructor(InviteCourseInstructorRequest inviteCourseInstructorRequest)
        {
            try
            {
                var courseInstructor = new CourseInstructor
                {
                    CourseId = inviteCourseInstructorRequest.CourseId,
                    InstructorId = inviteCourseInstructorRequest.InstructorId,
                    IsPrimaryInstructor = false
                };
                await _courseInstructorRepository.AddAsync(courseInstructor);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseInstructor: " + ex.Message);
            }
        }

        public async Task RemoveAccessCourseInstructor(RemoveCourseInstructorRequest removeCourseInstructorRequest)
        {
            try
            {
                var courseInstructor = await GetCourseInstructor(removeCourseInstructorRequest.CourseId, removeCourseInstructorRequest.InstructorId);
                if (courseInstructor == null)
                {
                    throw new Exception("CourseInstructor not found.");
                }
                await _courseInstructorRepository.DeleteAsync(courseInstructor);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the courseInstructor: " + ex.Message);
            }
        }

        public async Task UpdateCourseInstructor(CourseInstructor courseInstructor)
        {
            try
            {
                await _courseInstructorRepository.UpdateAsync(courseInstructor);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the courseInstructor: " + ex.Message);
            }
        }

        public async Task DeleteCourseInstructor(CourseInstructor courseInstructor)
        {
            try
            {
                await _courseInstructorRepository.DeleteAsync(courseInstructor);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the courseInstructor: " + ex.Message);
            }
        }
    }
}
