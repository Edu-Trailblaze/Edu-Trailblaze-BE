using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ICourseService _courseService;
        private readonly IAIService _aiService;
        private readonly IRepository<Tag, int> _tagRepository;
        private readonly IRepository<UserTag, int> _userTagRepository;

        public AdminDashboardService(ICourseService courseService, IAIService aiService, IRepository<Tag, int> tagRepository, IRepository<UserTag, int> userTagRepository)
        {
            _courseService = courseService;
            _aiService = aiService;
            _tagRepository = tagRepository;
            _userTagRepository = userTagRepository;
        }

        public async Task ApproveCourse(ApproveCourseRequest request)
        {
            try
            {
                var course = await _courseService.GetCourse(request.CourseId);

                if (course == null)
                {
                    throw new Exception("Course not found.");
                }

                course.ApprovalStatus = request.Status;
                course.IsPublished = request.Status == "Approved";
                await _courseService.UpdateCourse(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving the course: " + ex.Message);
            }
        }

        public async Task ApproveCourseByAI(int courseId)
        {
            try
            {
                var course = await _courseService.GetCourse(courseId);

                if (course == null)
                {
                    throw new Exception("Course not found.");
                }

                var courseDetectAIRequest = new CourseDetectionRequest
                {
                    title = course.Title,
                    description = course.Description,
                };

                var tags = await _aiService.CourseDetectionAIV2(courseDetectAIRequest);

                if (tags == null || !tags.Any())
                {
                    throw new Exception("AI response is null or empty.");
                }

                var tagDbSet = await _tagRepository.GetDbSet();
                var userTagDbSet = await _userTagRepository.GetDbSet();

                var hasTag = await userTagDbSet
                    .Where(ut => ut.UserId == course.CreatedBy)
                    .AnyAsync(ut => tags.AsQueryable().Contains(ut.Tag.Name));

                if (!hasTag)
                {
                    course.IsPublished = false;
                    course.ApprovalStatus = "Rejected";
                }
                else
                {
                    course.IsPublished = true;
                    course.ApprovalStatus = "Approved";
                }

                await _courseService.UpdateCourse(course);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while approving the course by AI: " + ex.Message);
            }
        }
    }
}
