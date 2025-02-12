using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ISectionService
    {
        Task<Section?> GetSection(int sectionId);

        Task<IEnumerable<Section>> GetSections();

        Task AddSection(Section section);

        Task UpdateSection(Section section);

        Task DeleteSection(Section section);

        Task AddSection(CreateSectionRequest section);

        Task UpdateSection(UpdateSectionRequest section);

        Task DeleteSection(int section);

        Task UpdateSectionDuration(int sectionId);

        Task UpdateNumberOfLectures(int sectionId);

        Task<List<SectionDTO>?> GetSectionsByConditions(GetSectionsRequest request);
    }
}
