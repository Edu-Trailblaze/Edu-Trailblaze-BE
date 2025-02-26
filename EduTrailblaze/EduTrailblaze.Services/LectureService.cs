using AutoMapper;
using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class LectureService : ILectureService
    {
        private readonly IRepository<Lecture, int> _lectureRepository;
        private readonly ISectionService _sectionService;
        private readonly IMapper _mapper;

        public LectureService(IRepository<Lecture, int> lectureRepository, ISectionService sectionService, IMapper mapper)
        {
            _lectureRepository = lectureRepository;
            _sectionService = sectionService;
            _mapper = mapper;
        }

        public async Task<Lecture?> GetLecture(int lectureId)
        {
            try
            {
                return await _lectureRepository.GetByIdAsync(lectureId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the lecture.", ex);
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
                throw new Exception("An error occurred while getting the lecture.", ex);
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
                throw new Exception("An error occurred while adding the lecture.", ex);
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
                throw new Exception("An error occurred while updating the lecture.", ex);
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
        //        throw new Exception("An error occurred while adding the lecture.", ex);
        //    }
        //}
        
        public async Task<LectureDTO> CreateLecture(CreateLecture lecture)
        {
            try
            {
                string? content = null;

                if (lecture.ContentPDFFile != null && lecture.ContentPDFFile.Length > 0)
                {
                    content = PDFReader.ExtractText(lecture.ContentPDFFile);
                }

                var lectureEntity = new Lecture
                {
                    SectionId = lecture.SectionId,
                    LectureType = lecture.LectureType,
                    Title = lecture.Title,
                    Content = content ?? lecture.Content,
                    Description = lecture.Description,
                    Duration = lecture.Duration ?? 0,
                };
                await _lectureRepository.AddAsync(lectureEntity);
                await UpdateLectureDuration(lecture.SectionId);
                await _sectionService.UpdateNumberOfLectures(lecture.SectionId);

                return _mapper.Map<LectureDTO>(lectureEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the lecture.", ex);
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
                await UpdateLectureDuration(lecture.SectionId);
                await _sectionService.UpdateNumberOfLectures(lecture.SectionId);

                return _mapper.Map<LectureDTO>(lectureEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the lecture.", ex);
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
                throw new Exception("An error occurred while updating the lecture.", ex);
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
                throw new Exception("An error occurred while deleting the lecture.", ex);
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
                throw new Exception("An error occurred while deleting the lecture.", ex);
            }
        }

        public async Task UpdateLectureDuration(int lectureId)
        {
            try
            {
                var lectureDbSet = await _lectureRepository.GetDbSet();

                var lecture = await lectureDbSet
                    .Include(x => x.Videos)
                    .FirstOrDefaultAsync(x => x.Id == lectureId);
                if (lecture == null)
                {
                    throw new Exception("Lecture not found.");
                }

                lecture.Duration = (int)Math.Ceiling(lecture.Videos.Where(v => !v.IsDeleted)
                                                                   .Sum(v => v.Duration.TotalMinutes));

                await _lectureRepository.UpdateAsync(lecture);
                await _sectionService.UpdateSectionDuration(lecture.SectionId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the lecture duration.", ex);
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
    }
}
