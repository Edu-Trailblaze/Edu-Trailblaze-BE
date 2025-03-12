using AutoMapper;
using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class LectureService : ILectureService
    {
        private readonly IRepository<Lecture, int> _lectureRepository;
        private readonly ISectionService _sectionService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public LectureService(IRepository<Lecture, int> lectureRepository, ISectionService sectionService, IMapper mapper, ICourseService courseService, ICloudinaryService cloudinaryService)
        {
            _lectureRepository = lectureRepository;
            _sectionService = sectionService;
            _mapper = mapper;
            _courseService = courseService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Lecture?> GetLecture(int lectureId)
        {
            try
            {
                return await _lectureRepository.GetByIdAsync(lectureId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the lecture: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Lecture>> GetLectures()
        {
            try
            {
                return await _lectureRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the lecture: " + ex.Message);
            }
        }

        public async Task AddLecture(Lecture lecture)
        {
            try
            {
                await _lectureRepository.AddAsync(lecture);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the lecture: " + ex.Message);
            }
        }

        public async Task UpdateLecture(Lecture lecture)
        {
            try
            {
                await _lectureRepository.UpdateAsync(lecture);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the lecture: " + ex.Message);
            }
        }

        //public async Task CreateLecture(CreateLectureDetails lecture)
        //{
        //    try
        //    {
        //        var lectureEntity = new Lecture
        //        {
        //            SectionId = lecture.SectionId,
        //            LectureType = lecture.LectureType,
        //            Title = lecture.Title,
        //            Content = lecture.Content,
        //            Description = lecture.Description,
        //            Duration = lecture.Duration ?? 0,
        //        };
        //        await _lectureRepository.AddAsync(lectureEntity);
        //        await UpdateLectureDuration(lecture.SectionId);
        //        await _sectionService.UpdateNumberOfLectures(lecture.SectionId);

        //        if (lecture.Video != null)
        //        {
        //            lecture.Video.LectureId = lectureEntity.Id;
        //            await UploadVideoWithCloudinaryAsync(lecture.Video);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while adding the lecture: " + ex.Message);
        //    }
        //}

        public async Task<LectureDTO> CreateLecture(CreateLecture lecture)
        {
            try
            {
                var lectureEntity = new Lecture
                {
                    SectionId = lecture.SectionId,
                    LectureType = lecture.LectureType,
                    Title = lecture.Title,
                    Content = lecture.Content,
                    Description = lecture.Description,
                    Duration = lecture.Duration ?? 0
                };
                await _lectureRepository.AddAsync(lectureEntity);

                if (lecture.ContentFile != null && lecture.ContentFile.Length > 0)
                {
                    var fileExtension = Path.GetExtension(lecture.ContentFile.FileName);
                    var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{fileExtension}");
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await lecture.ContentFile.CopyToAsync(stream);
                    }

                    lectureEntity.DocUrl = await _cloudinaryService.UploadFileAsync(tempFilePath, lectureEntity.Id.ToString() + fileExtension);

                    File.Delete(tempFilePath);

                    await _lectureRepository.UpdateAsync(lectureEntity);
                }

                await UpdateLectureDuration(lectureEntity.Id);
                await _sectionService.UpdateNumberOfLectures(lecture.SectionId);
                var sec = await _sectionService.GetSection(lecture.SectionId);

                if (sec != null) await _courseService.CheckAndUpdateCourseContent(sec.CourseId);

                return _mapper.Map<LectureDTO>(lectureEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the lecture: " + ex.Message);
            }
        }

        public async Task<LectureDTO> AddLecture(CreateLectureRequest lecture)
        {
            try
            {
                var lectureEntity = new Lecture
                {
                    SectionId = lecture.SectionId,
                    LectureType = lecture.LectureType,
                    Title = lecture.Title,
                    Content = lecture.Content,
                    Description = lecture.Description,
                    Duration = lecture.Duration ?? 0,
                };
                await _lectureRepository.AddAsync(lectureEntity);

                var updateLectureDurationTask = UpdateLectureDuration(lecture.SectionId);
                var updateNumberOfLecturesTask = _sectionService.UpdateNumberOfLectures(lecture.SectionId);
                var checkAndUpdateCourseContentTask = _courseService.CheckAndUpdateCourseContent(lecture.SectionId);

                await Task.WhenAll(updateLectureDurationTask, updateNumberOfLecturesTask, checkAndUpdateCourseContentTask);

                return _mapper.Map<LectureDTO>(lectureEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the lecture: " + ex.Message);
            }
        }

        public async Task UpdateLecture(UpdateLectureRequest lecture)
        {
            try
            {
                var lectureEntity = await _lectureRepository.GetByIdAsync(lecture.LectureId);
                if (lectureEntity == null)
                {
                    throw new Exception("Lecture not found.");
                }
                lectureEntity.SectionId = lecture.SectionId;
                lectureEntity.LectureType = lecture.LectureType;
                lectureEntity.Title = lecture.Title;
                lectureEntity.Content = lecture.Content;
                lectureEntity.Description = lecture.Description;
                lectureEntity.Duration = lecture.Duration;
                lectureEntity.UpdatedAt = DateTimeHelper.GetVietnamTime();

                await _lectureRepository.UpdateAsync(lectureEntity);

                await UpdateLectureDuration(lecture.SectionId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the lecture: " + ex.Message);
            }
        }

        public async Task DeleteLecture(Lecture lecture)
        {
            try
            {
                await _lectureRepository.DeleteAsync(lecture);
                await _sectionService.UpdateNumberOfLectures(lecture.SectionId);
                await UpdateLectureDuration(lecture.SectionId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the lecture: " + ex.Message);
            }
        }

        public async Task DeleteLecture(int lectureId)
        {
            try
            {
                var lecture = await _lectureRepository.GetByIdAsync(lectureId);
                if (lecture == null)
                {
                    throw new Exception("Lecture not found.");
                }

                lecture.IsDeleted = true;

                await _lectureRepository.UpdateAsync(lecture);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the lecture: " + ex.Message);
            }
        }

        public async Task UpdateLectureDuration(int lectureId)
        {
            try
            {
                var lectureDbSet = await _lectureRepository.GetDbSet();

                var lecture = await lectureDbSet
                    .Include(x => x.Videos)
                    .FirstOrDefaultAsync(x => x.Id == lectureId && x.LectureType == "Video");
                if (lecture == null)
                {
                    return;
                }

                lecture.Duration = (int)Math.Ceiling(lecture.Videos.Where(v => !v.IsDeleted)
                                                                   .Sum(v => v.Duration.TotalMinutes));

                await _lectureRepository.UpdateAsync(lecture);
                await _sectionService.UpdateSectionDuration(lecture.SectionId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the lecture duration: " + ex.Message);
            }
        }

        public async Task<List<LectureDTO>?> GetLecturesByConditions(GetLecturesRequest request)
        {
            try
            {
                var dbSet = await _lectureRepository.GetDbSet();

                if (request.IsDeleted != null)
                {
                    dbSet = dbSet.Where(c => c.IsDeleted == request.IsDeleted);
                }

                if (request.SectionId != null)
                {
                    dbSet = dbSet.Where(c => c.SectionId == request.SectionId);
                }

                if (!string.IsNullOrEmpty(request.Title))
                {
                    dbSet = dbSet.Where(c => c.Title.ToLower().Contains(request.Title.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.Description))
                {
                    dbSet = dbSet.Where(c => c.Description.ToLower().Contains(request.Description.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.Content))
                {
                    dbSet = dbSet.Where(c => c.Content.ToLower().Contains(request.Content.ToLower()));
                }

                if (request.MinDuration != null)
                {
                    dbSet = dbSet.Where(c => c.Duration >= request.MinDuration);
                }

                if (request.MaxDuration != null)
                {
                    dbSet = dbSet.Where(c => c.Duration <= request.MaxDuration);
                }

                var lectures = await dbSet.ToListAsync();

                return _mapper.Map<List<LectureDTO>>(lectures);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the lectures: " + ex.Message);
            }
        }

        public async Task<List<SectionLectureDetails>> GetSectionLectures(List<int> sectionIds)
        {
            try
            {
                List<SectionLectureDetails> sectionLectureDetails1 = new List<SectionLectureDetails>();
                foreach (var sectionId in sectionIds)
                {
                    var dbSet = await _lectureRepository.GetDbSet();
                    var sectionLectureDetails = new SectionLectureDetails
                    {
                        SectionId = sectionId,
                        Lectures = await dbSet.Where(x => x.SectionId == sectionId && !x.IsDeleted)
                            .Select(x => _mapper.Map<LectureDTO>(x)).ToListAsync()
                    };
                    sectionLectureDetails1.Add(sectionLectureDetails);
                }

                return sectionLectureDetails1;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the lectures: " + ex.Message);
            }
        }

        public async Task<ApiResponse> CreateListSectionLectures(CreateListSectionLectureRequest sectionLecture)
        {
            try
            {
                foreach (var section in sectionLecture.Sections)
                {
                    var sectionEntity = new Section
                    {
                        CourseId = sectionLecture.CourseId,
                        Title = section.Title,
                        Description = section.Description,
                        NumberOfLectures = 0,
                        Duration = TimeSpan.FromMinutes(section.Lectures.Sum(l => l.Duration ?? 0))
                    };

                    await _sectionService.AddSection(sectionEntity);

                    foreach (var lecture in section.Lectures)
                    {
                        var createLecture = new CreateLecture
                        {
                            SectionId = sectionEntity.Id,
                            LectureType = lecture.LectureType,
                            Title = lecture.Title,
                            ContentFile = lecture.ContentFile,
                            Content = lecture.Content,
                            Description = lecture.Description,
                            Duration = lecture.Duration
                        };

                        await CreateLecture(createLecture);
                    }

                    await _sectionService.UpdateSectionDuration(sectionEntity.Id);
                }

                return new ApiResponse
                {
                    StatusCode = 200,
                    Message = "Sections and lectures created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = "An error occurred while creating sections and lectures: " + ex.Message
                };
            }
        }
    }
}
