using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class UserProgressService : IUserProgressService
    {
        private readonly IRepository<UserProgress, int> _userProgressRepository;
        private readonly IRepository<Lecture, int> _lectureRepository;
        private readonly IEnrollmentService _enrollmentService;

        public UserProgressService(IRepository<UserProgress, int> userProgressRepository, IEnrollmentService enrollmentService, IRepository<Lecture, int> lectureRepository)
        {
            _userProgressRepository = userProgressRepository;
            _enrollmentService = enrollmentService;
            _lectureRepository = lectureRepository;
        }

        public async Task<UserProgress?> GetUserProgress(int userProgressId)
        {
            try
            {
                return await _userProgressRepository.GetByIdAsync(userProgressId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userProgress.", ex);
            }
        }

        public async Task<IEnumerable<UserProgress>> GetUserProgresss()
        {
            try
            {
                return await _userProgressRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userProgress.", ex);
            }
        }

        public async Task AddUserProgress(UserProgress userProgress)
        {
            try
            {
                await _userProgressRepository.AddAsync(userProgress);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the userProgress.", ex);
            }
        }

        public async Task AddUserProgress(ProgressRequest request)
        {
            try
            {
                if (request.SectionId != null)
                {
                    var userProgressDb = await (await _userProgressRepository.GetDbSet())
                        .Where(up => up.UserId == request.UserId && up.SectionId == request.SectionId)
                        .FirstOrDefaultAsync();
                    if (userProgressDb == null)
                    {
                        var userProgress = new UserProgress
                        {
                            UserId = request.UserId,
                            CourseClassId = request.CourseClassId,
                            SectionId = request.SectionId,
                            ProgressType = "Section",
                            LastAccessed = DateTimeHelper.GetVietnamTime()
                        };
                        await _userProgressRepository.AddAsync(userProgress);
                    }
                }
                else if (request.LectureId != null)
                {
                    var userProgressDb = await (await _userProgressRepository.GetDbSet())
                        .Where(up => up.UserId == request.UserId && up.LectureId == request.LectureId)
                        .FirstOrDefaultAsync();
                    if (userProgressDb == null)
                    {
                        var userProgress = new UserProgress
                        {
                            UserId = request.UserId,
                            CourseClassId = request.CourseClassId,
                            LectureId = request.LectureId,
                            ProgressType = "Lecture",
                            LastAccessed = DateTimeHelper.GetVietnamTime()
                        };
                        await _userProgressRepository.AddAsync(userProgress);
                    }
                }
                else if (request.QuizId != null)
                {
                    var userProgressDb = await (await _userProgressRepository.GetDbSet())
                        .Where(up => up.UserId == request.UserId && up.QuizId == request.QuizId)
                        .FirstOrDefaultAsync();
                    if (userProgressDb == null)
                    {
                        var userProgress = new UserProgress
                        {
                            UserId = request.UserId,
                            CourseClassId = request.CourseClassId,
                            QuizId = request.QuizId,
                            ProgressType = "Quiz",
                            LastAccessed = DateTimeHelper.GetVietnamTime()
                        };
                        await _userProgressRepository.AddAsync(userProgress);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the userProgress.", ex);
            }
        }

        public async Task SaveUserProgress(SaveUserProgressRequest userProgressRequest)
        {
            try
            {
                var lectureDbSet = await _lectureRepository.GetDbSet();

                var courseId = await lectureDbSet
                    .Where(l => l.Id == userProgressRequest.LectureId)
                    .Select(l => l.Section.CourseId)
                    .FirstOrDefaultAsync();

                var courseClassId = await _enrollmentService.GetStudentCourseClass(userProgressRequest.UserId, courseId);

                var userProgress = new UserProgress
                {
                    UserId = userProgressRequest.UserId,
                    CourseClassId = courseClassId,
                    LectureId = userProgressRequest.LectureId,
                    ProgressType = "Lecture",
                    ProgressPercentage = 100,
                    IsCompleted = true,
                    LastAccessed = DateTimeHelper.GetVietnamTime()
                };

                await _userProgressRepository.AddAsync(userProgress);

                var lecture = await (await _userProgressRepository.GetDbSet())
                    .Where(up => up.LectureId == userProgressRequest.LectureId)
                    .Select(up => up.Lecture)
                    .FirstOrDefaultAsync();

                if (lecture != null)
                {
                    var sectionId = lecture.SectionId;
                    var allLecturesInSection = await (await _userProgressRepository.GetDbSet())
                        .Where(up => up.SectionId == sectionId && up.UserId == userProgressRequest.UserId)
                        .ToListAsync();

                    var allLecturesCompleted = allLecturesInSection.All(up => up.IsCompleted);

                    if (allLecturesCompleted)
                    {
                        var sectionProgress = new UserProgress
                        {
                            UserId = userProgressRequest.UserId,
                            SectionId = sectionId,
                            ProgressType = "Section",
                            ProgressPercentage = 100,
                            IsCompleted = true,
                            LastAccessed = DateTimeHelper.GetVietnamTime()
                        };

                        await _userProgressRepository.AddAsync(sectionProgress);

                        var section = await (await _userProgressRepository.GetDbSet())
                            .Where(up => up.SectionId == sectionId)
                            .Select(up => up.Section)
                            .FirstOrDefaultAsync();

                        if (section != null)
                        {
                            var allSectionsInCourse = await (await _userProgressRepository.GetDbSet())
                                .Where(up => up.CourseClassId == courseClassId && up.UserId == userProgressRequest.UserId)
                                .ToListAsync();

                            var allSectionsCompleted = allSectionsInCourse.All(up => up.IsCompleted);

                            if (allSectionsCompleted)
                            {
                                var courseProgress = new UserProgress
                                {
                                    UserId = userProgressRequest.UserId,
                                    CourseClassId = courseClassId,
                                    ProgressType = "Course",
                                    ProgressPercentage = 100,
                                    IsCompleted = true,
                                    LastAccessed = DateTimeHelper.GetVietnamTime()
                                };

                                await _userProgressRepository.AddAsync(courseProgress);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the userProgress.", ex);
            }
        }

        public async Task UpdateUserProgress(UserProgress userProgress)
        {
            try
            {
                await _userProgressRepository.UpdateAsync(userProgress);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the userProgress.", ex);
            }
        }

        public async Task UpdateUserProgress(UpdateUserProgress userProgress)
        {
            try
            {
                var userProgressDb = await _userProgressRepository.GetByIdAsync(userProgress.Id);
                if (userProgressDb == null)
                {
                    throw new Exception("UserProgress not found.");
                }
                userProgressDb.ProgressPercentage = userProgress.ProgressPercentage;
                userProgressDb.IsCompleted = userProgress.IsCompleted;
                userProgressDb.LastAccessed = DateTimeHelper.GetVietnamTime();
                await _userProgressRepository.UpdateAsync(userProgressDb);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the userProgress.", ex);
            }
        }

        public async Task DeleteUserProgress(UserProgress userProgress)
        {
            try
            {
                await _userProgressRepository.DeleteAsync(userProgress);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the userProgress.", ex);
            }
        }

        public async Task<UserProgress?> GetUserProgress(string userId, int? sectionId, int? lectureId, int? quizId)
        {
            try
            {
                var userProgress = await (await _userProgressRepository.GetDbSet())
                    .Where(up => up.UserId == userId)
                    .Where(up => up.SectionId == sectionId || up.LectureId == lectureId || up.QuizId == quizId)
                    .FirstOrDefaultAsync();
                return userProgress;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userProgress.", ex);
            }
        }
    }
}
