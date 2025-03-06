using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class QuizAnswerService : IQuizAnswerService
    {
        private readonly IRepository<QuizAnswer, int> _quizAnswerRepository;

        public QuizAnswerService(IRepository<QuizAnswer, int> quizAnswerRepository)
        {
            _quizAnswerRepository = quizAnswerRepository;
        }

        public async Task<QuizAnswer?> GetQuizAnswer(int quizAnswerId)
        {
            try
            {
                return await _quizAnswerRepository.GetByIdAsync(quizAnswerId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the quizAnswer: " + ex.Message);
            }
        }

        public async Task<IEnumerable<QuizAnswer>> GetQuizAnswers()
        {
            try
            {
                return await _quizAnswerRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the quizAnswer: " + ex.Message);
            }
        }

        public async Task AddQuizAnswer(QuizAnswer quizAnswer)
        {
            try
            {
                await _quizAnswerRepository.AddAsync(quizAnswer);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the quizAnswer: " + ex.Message);
            }
        }

        public async Task UpdateQuizAnswer(QuizAnswer quizAnswer)
        {
            try
            {
                await _quizAnswerRepository.UpdateAsync(quizAnswer);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the quizAnswer: " + ex.Message);
            }
        }

        public async Task DeleteQuizAnswer(QuizAnswer quizAnswer)
        {
            try
            {
                await _quizAnswerRepository.DeleteAsync(quizAnswer);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the quizAnswer: " + ex.Message);
            }
        }
    }
}
