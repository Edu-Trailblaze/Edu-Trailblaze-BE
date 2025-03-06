using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag, int> _tagRepository;

        public TagService(IRepository<Tag, int> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<Tag?> GetTag(int tagId)
        {
            try
            {
                return await _tagRepository.GetByIdAsync(tagId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the tag: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Tag>> GetTags()
        {
            try
            {
                return await _tagRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the tag: " + ex.Message);
            }
        }

        public async Task AddTag(Tag tag)
        {
            try
            {
                await _tagRepository.AddAsync(tag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the tag: " + ex.Message);
            }
        }

        public async Task UpdateTag(Tag tag)
        {
            try
            {
                await _tagRepository.UpdateAsync(tag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the tag: " + ex.Message);
            }
        }

        public async Task DeleteTag(Tag tag)
        {
            try
            {
                await _tagRepository.DeleteAsync(tag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the tag: " + ex.Message);
            }
        }

        public async Task AddTag(CreateTagRequest tag)
        {
            try
            {
                var newTag = new Tag
                {
                    Name = tag.Name,
                    Description = tag.Description
                };
                await _tagRepository.AddAsync(newTag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the tag: " + ex.Message);
            }
        }

        public async Task UpdateTag(UpdateTagRequest tag)
        {
            try
            {
                var existingTag = await _tagRepository.GetByIdAsync(tag.TagId);
                if (existingTag == null)
                {
                    throw new Exception("Tag not found.");
                }
                existingTag.Name = tag.Name;
                existingTag.Description = tag.Description;
                existingTag.UpdatedAt = DateTimeHelper.GetVietnamTime();
                await _tagRepository.UpdateAsync(existingTag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the tag: " + ex.Message);
            }
        }

        public async Task DeleteTag(int tag)
        {
            try
            {
                var existingTag = await _tagRepository.GetByIdAsync(tag);
                if (existingTag == null)
                {
                    throw new Exception("Tag not found.");
                }
                await _tagRepository.DeleteAsync(existingTag);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the tag: " + ex.Message);
            }
        }
    }
}
