using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IQuizService
    {
        Task<Quiz?> GetQuiz(int quizId);

        Task<IEnumerable<Quiz>> GetQuizs();

        Task AddQuiz(Quiz quiz);

        Task UpdateQuiz(Quiz quiz);

        Task DeleteQuiz(Quiz quiz);

        Task<QuizDTO> AddQuiz(CreateQuizRequest quiz);

        Task UpdateQuiz(UpdateQuizRequest quiz);

        Task DeleteQuiz(int quiz);

        Task<QuizDetails> GetQuizDetails(int lectureId);

        Task CreateQuizDetails(CreateQuizDetails request);
    }
}
