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

                var aiResponse = await _aiService.CourseDetectionAI(courseDetectAIRequest);

                if (aiResponse == null)
                {
                    throw new Exception("AI response is null.");
                }

                var tagName = aiResponse.predicted_label;

                var tagDbSet = await _tagRepository.GetDbSet();
                var tag = await tagDbSet.FirstOrDefaultAsync(t => t.Name == tagName);

                if (tag == null)
                {
                    throw new Exception("Tag not found.");
                }
                var userTagDbSet = await _userTagRepository.GetDbSet();

                var hasTag = await userTagDbSet.AnyAsync(ut => ut.UserId == course.CreatedBy && ut.TagId == tag.Id);

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
