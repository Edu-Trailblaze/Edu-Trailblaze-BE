using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IRepository<Language, int> _languageRepository;

        public LanguageService(IRepository<Language, int> languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public async Task<Language?> GetLanguage(int languageId)
        {
            try
            {
                return await _languageRepository.GetByIdAsync(languageId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the language: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Language>> GetLanguages()
        {
            try
            {
                return await _languageRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the language: " + ex.Message);
            }
        }

        public async Task AddLanguage(Language language)
        {
            try
            {
                await _languageRepository.AddAsync(language);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the language: " + ex.Message);
            }
        }

        public async Task UpdateLanguage(Language language)
        {
            try
            {
                await _languageRepository.UpdateAsync(language);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the language: " + ex.Message);
            }
        }

        public async Task AddLanguage(CreateLanguageRequest language)
        {
            try
            {
                var languageEntity = new Language
                {
                    Name = language.Name,
                    Code = language.Code
                };

                await _languageRepository.AddAsync(languageEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the language: " + ex.Message);
            }
        }

        public async Task UpdateLanguage(UpdateLanguageRequest language)
        {
            try
            {
                var languageEntity = await _languageRepository.GetByIdAsync(language.LanguageId);
                if (languageEntity == null)
                {
                    throw new Exception("Language not found.");
                }
                languageEntity.Name = language.Name;
                languageEntity.Code = language.Code;
                await _languageRepository.UpdateAsync(languageEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the language: " + ex.Message);
            }
        }

        public async Task DeleteLanguage(Language language)
        {
            try
            {
                await _languageRepository.DeleteAsync(language);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the language: " + ex.Message);
            }
        }
    }
}
