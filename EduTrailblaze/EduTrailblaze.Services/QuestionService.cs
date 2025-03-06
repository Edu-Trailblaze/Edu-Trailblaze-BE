using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IRepository<Question, int> _questionRepository;

        public QuestionService(IRepository<Question, int> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<Question?> GetQuestion(int questionId)
        {
            try
            {
                return await _questionRepository.GetByIdAsync(questionId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the question: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Question>> GetQuestions()
        {
            try
            {
                return await _questionRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the question: " + ex.Message);
            }
        }

        public async Task AddQuestion(Question question)
        {
            try
            {
                await _questionRepository.AddAsync(question);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the question: " + ex.Message);
            }
        }

        public async Task UpdateQuestion(Question question)
        {
            try
            {
                await _questionRepository.UpdateAsync(question);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the question: " + ex.Message);
            }
        }

        public async Task AddQuestion(CreateQuestionRequest question)
        {
            try
            {
                var newQuestion = new Question
                {
                    QuizzId = question.QuizzId,
                    QuestionText = question.QuestionText
                };
                await _questionRepository.AddAsync(newQuestion);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the question: " + ex.Message);
            }
        }

        public async Task UpdateQuestion(UpdateQuestionRequest question)
        {
            try
            {
                var questionToUpdate = await _questionRepository.GetByIdAsync(question.QuestionId);
                if (questionToUpdate == null)
                {
                    throw new Exception("Question not found.");
                }
                questionToUpdate.QuestionText = question.QuestionText;
                questionToUpdate.UpdatedAt = DateTimeHelper.GetVietnamTime();
                await _questionRepository.UpdateAsync(questionToUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the question: " + ex.Message);
            }
        }

        public async Task DeleteQuestion(Question question)
        {
            try
            {
                await _questionRepository.DeleteAsync(question);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the question: " + ex.Message);
            }
        }

        public async Task DeleteQuestion(int questionId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null)
                {
                    throw new Exception("Question not found.");
                }
                await _questionRepository.DeleteAsync(question);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the question: " + ex.Message);
            }
        }
    }
}
