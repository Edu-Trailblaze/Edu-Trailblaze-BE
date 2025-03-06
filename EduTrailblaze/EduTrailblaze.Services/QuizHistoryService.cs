using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class QuizHistoryService : IQuizHistoryService
    {
        private readonly IRepository<QuizHistory, int> _quizHistoryRepository;

        public QuizHistoryService(IRepository<QuizHistory, int> quizHistoryRepository)
        {
            _quizHistoryRepository = quizHistoryRepository;
        }

        public async Task<QuizHistory?> GetQuizHistory(int quizHistoryId)
        {
            try
            {
                return await _quizHistoryRepository.GetByIdAsync(quizHistoryId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the quizHistory: " + ex.Message);
            }
        }

        public async Task<IEnumerable<QuizHistory>> GetQuizHistorys()
        {
            try
            {
                return await _quizHistoryRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the quizHistory: " + ex.Message);
            }
        }

        public async Task AddQuizHistory(QuizHistory quizHistory)
        {
            try
            {
                await _quizHistoryRepository.AddAsync(quizHistory);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the quizHistory: " + ex.Message);
            }
        }

        public async Task UpdateQuizHistory(QuizHistory quizHistory)
        {
            try
            {
                await _quizHistoryRepository.UpdateAsync(quizHistory);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the quizHistory: " + ex.Message);
            }
        }

        public async Task DeleteQuizHistory(QuizHistory quizHistory)
        {
            try
            {
                await _quizHistoryRepository.DeleteAsync(quizHistory);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the quizHistory: " + ex.Message);
            }
        }
    }
}
