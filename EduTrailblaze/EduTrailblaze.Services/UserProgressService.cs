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
        private readonly IRepository<Section, int> _sectionRepository;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUserCertificateService _userCertificateService;

        public UserProgressService(IRepository<UserProgress, int> userProgressRepository, IEnrollmentService enrollmentService, IRepository<Lecture, int> lectureRepository, IRepository<Section, int> sectionRepository, IUserCertificateService userCertificateService)
        {
            _userProgressRepository = userProgressRepository;
            _enrollmentService = enrollmentService;
            _lectureRepository = lectureRepository;
            _sectionRepository = sectionRepository;
            _userCertificateService = userCertificateService;
        }

        public async Task<UserProgress?> GetUserProgress(int userProgressId)
        {
            try
            {
                return await _userProgressRepository.GetByIdAsync(userProgressId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userProgress: " + ex.Message);
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
                throw new Exception("An error occurred while getting the userProgress: " + ex.Message);
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
                throw new Exception("An error occurred while adding the userProgress: " + ex.Message);
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
                throw new Exception("An error occurred while adding the userProgress: " + ex.Message);
            }
        }

        public async Task SaveUserProgress(SaveUserProgressRequest userProgressRequest)
        {
            try
            {
                var lecture = await _lectureRepository.GetByIdAsync(userProgressRequest.LectureId);
                if (lecture == null)
                {
                    throw new Exception("Lecture not found.");
                }

                var lectureDbSet = await _lectureRepository.GetDbSet();

                var courseId = await lectureDbSet
                    .Where(l => l.Id == userProgressRequest.LectureId)
                    .Select(l => l.Section.CourseId)
                    .FirstOrDefaultAsync();

                var courseClassId = await _enrollmentService.GetStudentCourseClass(userProgressRequest.UserId, courseId);

                var checkLectureProgress = await (await _userProgressRepository.GetDbSet())
                    .Where(up => up.LectureId == userProgressRequest.LectureId && up.UserId == userProgressRequest.UserId)
                    .FirstOrDefaultAsync();

                if (checkLectureProgress != null)
                {
                    return;
                }

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

                var sectionId = lecture.SectionId;
                var allLecturesInSection = await lectureDbSet
                    .Where(l => l.SectionId == sectionId)
                    .ToListAsync();

                var userProgressDbSet = await _userProgressRepository.GetDbSet();
                var allLecturesCompleted = true;

                foreach (var lectureItem in allLecturesInSection)
                {
                    var lectureItemProgress = await userProgressDbSet
                        .FirstOrDefaultAsync(up => up.LectureId == lectureItem.Id && up.UserId == userProgressRequest.UserId);

                    if (lectureItemProgress == null || !lectureItemProgress.IsCompleted)
                    {
                        allLecturesCompleted = false;
                        break;
                    }
                }

                if (allLecturesCompleted)
                {
                    var checkSectionProgress = await userProgressDbSet
                        .FirstOrDefaultAsync(up => up.SectionId == sectionId && up.UserId == userProgressRequest.UserId);
                    if (checkSectionProgress == null)
                    {
                        var sectionProgress = new UserProgress
                        {
                            UserId = userProgressRequest.UserId,
                            CourseClassId = courseClassId,
                            SectionId = sectionId,
                            ProgressType = "Section",
                            ProgressPercentage = 100,
                            IsCompleted = true,
                            LastAccessed = DateTimeHelper.GetVietnamTime()
                        };

                        await _userProgressRepository.AddAsync(sectionProgress);
                    }

                    var section = await _sectionRepository.GetByIdAsync(sectionId);

                    var allSectionsInCourse = await (await _sectionRepository.GetDbSet())
                        .Where(s => s.CourseId == courseId)
                        .ToListAsync();

                    var allSectionsCompleted = true;

                    foreach (var sectionItem in allSectionsInCourse)
                    {
                        var sectionProgress = await userProgressDbSet
                            .FirstOrDefaultAsync(up => up.SectionId == sectionItem.Id && up.UserId == userProgressRequest.UserId && up.ProgressType == "Section");

                        if (sectionProgress == null || !sectionProgress.IsCompleted)
                        {
                            allSectionsCompleted = false;
                            break;
                        }
                    }

                    if (allSectionsCompleted)
                    {
                        var checkCourseProgress = await userProgressDbSet
                            .FirstOrDefaultAsync(up => up.CourseClassId == courseClassId && up.UserId == userProgressRequest.UserId && up.ProgressType == "Course");
                        if (checkCourseProgress == null)
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

                var courseProgressPercentage = await GetCourseProgress(userProgressRequest.UserId, courseId);
                var enrollment = await _enrollmentService.GetByCourseClassAndStudent(courseClassId, userProgressRequest.UserId);

                if (enrollment != null)
                {
                    enrollment.ProgressPercentage = courseProgressPercentage;
                    enrollment.IsCompleted = courseProgressPercentage >= 100;
                    enrollment.UpdatedAt = DateTimeHelper.GetVietnamTime();
                    await _enrollmentService.UpdateEnrollment(enrollment);

                    if (courseProgressPercentage >= 100 && enrollment.IsCompleted)
                    {
                        var userCertificate = new CreateUserCertificateRequest
                        {
                            UserId = userProgressRequest.UserId,
                            CourseId = courseId
                        };
                        //await Task.Run(async () => await _userCertificateService.AddUserCertificate(userCertificate));
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the user progress: " + ex.Message);
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
                throw new Exception("An error occurred while updating the userProgress: " + ex.Message);
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
                throw new Exception("An error occurred while updating the userProgress: " + ex.Message);
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
                throw new Exception("An error occurred while deleting the userProgress: " + ex.Message);
            }
        }

        public async Task DeleteUserProgress(int userProgressId)
        {
            try
            {
                var userProgress = await _userProgressRepository.GetByIdAsync(userProgressId);
                if (userProgress == null)
                {
                    throw new Exception("UserProgress not found.");
                }
                await _userProgressRepository.DeleteAsync(userProgress);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the userProgress: " + ex.Message);
            }
        }

        public async Task<List<UserProgress>?> GetUserProgress(string userId, int? sectionId, int? lectureId, int? quizId)
        {
            try
            {
                var userProgressQuery = (await _userProgressRepository.GetDbSet()).Where(up => up.UserId == userId);

                if (sectionId.HasValue)
                {
                    userProgressQuery = userProgressQuery.Where(up => up.SectionId == sectionId.Value);
                }

                if (lectureId.HasValue)
                {
                    userProgressQuery = userProgressQuery.Where(up => up.LectureId == lectureId.Value);
                }

                if (quizId.HasValue)
                {
                    userProgressQuery = userProgressQuery.Where(up => up.QuizId == quizId.Value);
                }

                var userProgress = await userProgressQuery.ToListAsync();
                return userProgress;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the userProgress: " + ex.Message);
            }
        }

        public async Task<decimal> GetCourseProgress(string userId, int courseId)
        {
            try
            {
                var courseClassId = await _enrollmentService.GetStudentCourseClass(userId, courseId);

                var totalLectures = await (await _lectureRepository.GetDbSet())
                    .Where(l => l.Section.CourseId == courseId && !l.IsDeleted)
                    .CountAsync();

                var completedLectures = await (await _userProgressRepository.GetDbSet())
                    .Where(up => up.CourseClassId == courseClassId && up.ProgressType == "Lecture" && up.IsCompleted)
                    .CountAsync();

                if (totalLectures == 0)
                {
                    return 0;
                }

                return Math.Round((decimal)completedLectures / totalLectures * 100, 2);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the course progress: " + ex.Message);
            }
        }
    }
}
