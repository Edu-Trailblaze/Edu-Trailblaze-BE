using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ILectureService
    {
        Task<Lecture?> GetLecture(int lectureId);

        Task<IEnumerable<Lecture>> GetLectures();

        Task AddLecture(Lecture lecture);

        Task UpdateLecture(Lecture lecture);

        Task<LectureDTO> AddLecture(CreateLectureRequest lecture);

        Task UpdateLecture(UpdateLectureRequest lecture);

        Task DeleteLecture(Lecture lecture);

        Task DeleteLecture(int lectureId);

        Task UpdateLectureDuration(int lectureId);

        Task<List<LectureDTO>?> GetLecturesByConditions(GetLecturesRequest request);

        Task<List<SectionLectureDetails>> GetSectionLectures(List<int> sectionIds);

        //Task CreateLecture(CreateLectureDetails lecture);

        Task<LectureDTO> CreateLecture(CreateLecture lecture);
    }
}
