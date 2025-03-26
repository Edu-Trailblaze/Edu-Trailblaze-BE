using AutoMapper;
using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class QuizService : IQuizService
    {
        private readonly IRepository<Quiz, int> _quizRepository;
        private readonly IRepository<Question, int> _questionRepository;
        private readonly IRepository<Lecture, int> _lectureRepository;
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public QuizService(IRepository<Quiz, int> quizRepository, IRepository<Question, int> questionRepository, IMapper mapper, IRepository<Lecture, int> lectureRepository, ICourseService courseService)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _mapper = mapper;
            _lectureRepository = lectureRepository;
            _courseService = courseService;
        }

        public async Task<Quiz?> GetQuiz(int quizId)
        {
            try
            {
                return await _quizRepository.GetByIdAsync(quizId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the quiz: " + ex.Message);
            }
        }

        public async Task<QuizDetails> GetQuizDetails(int lectureId)
        {
            try
            {
                // Retrieve the quiz associated with the given lectureId
                var quizDbSet = await _quizRepository.GetDbSet();

                var quiz = await quizDbSet.Include(q => q.Questions)
                  .ThenInclude(q => q.Answers)
                  .FirstOrDefaultAsync(q => q.LectureId == lectureId);

                if (quiz == null)
                {
                    throw new Exception("Quiz not found.");
                }

                // Map the quiz to QuizDetails
                var quizDetails = new QuizDetails
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    PassingScore = quiz.PassingScore,
                    Questions = quiz.Questions.Select(q => new QuestionDetails
                    {
                        Id = q.Id,
                        QuizzId = q.QuizzId,
                        QuestionText = q.QuestionText,
                        Answers = q.Answers.Select(a => new AnswerDetails
                        {
                            Id = a.Id,
                            QuestionId = a.QuestionId,
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect
                        }).ToList()
                    }).ToList()
                };

                return quizDetails;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the quiz: " + ex.Message);
            }
        }

        public async Task CreateQuizDetails(CreateQuizDetails request)
        {
            try
            {
                var lecture = await _lectureRepository.GetByIdAsync(request.LectureId);
                if (lecture == null)
                {
                    throw new Exception("Lecture not found.");
                }

                if (request.Questions == null || request.Questions.Count == 0)
                {
                    throw new Exception("Not found Questions");
                }

                CreateQuizRequest createQuizRequest = new CreateQuizRequest
                {
                    LectureId = request.LectureId,
                    Title = request.Title,
                    PassingScore = request.PassingScore
                };

                var quiz = await AddQuiz(createQuizRequest);

                foreach (var item in request.Questions)
                {
                    var question = new Question
                    {
                        QuizzId = quiz.Id,
                        QuestionText = item.QuestionText,
                        Answers = item.Answers.Select(a => new Answer
                        {
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect
                        }).ToList()
                    };

                    await _questionRepository.AddAsync(question);
                }

                await _courseService.CheckAndUpdateCourseContent(lecture.Section.CourseId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the question details: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Quiz>> GetQuizs()
        {
            try
            {
                return await _quizRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the quiz: " + ex.Message);
            }
        }

        public async Task AddQuiz(Quiz quiz)
        {
            try
            {
                await _quizRepository.AddAsync(quiz);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the quiz: " + ex.Message);
            }
        }

        public async Task UpdateQuiz(Quiz quiz)
        {
            try
            {
                await _quizRepository.UpdateAsync(quiz);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the quiz: " + ex.Message);
            }
        }

        public async Task DeleteQuiz(Quiz quiz)
        {
            try
            {
                await _quizRepository.DeleteAsync(quiz);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the quiz: " + ex.Message);
            }
        }

        public async Task<QuizDTO> AddQuiz(CreateQuizRequest quiz)
        {
            try
            {
                var newQuiz = new Quiz
                {
                    LectureId = quiz.LectureId,
                    Title = quiz.Title,
                    PassingScore = quiz.PassingScore
                };
                await _quizRepository.AddAsync(newQuiz);

                return _mapper.Map<QuizDTO>(newQuiz);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the quiz: " + ex.Message);
            }
        }

        public async Task UpdateQuiz(UpdateQuizRequest quiz)
        {
            try
            {
                var quizToUpdate = await _quizRepository.GetByIdAsync(quiz.QuizzId);
                if (quizToUpdate == null)
                {
                    throw new Exception("Quiz not found.");
                }
                quizToUpdate.Title = quiz.Title;
                quizToUpdate.PassingScore = quiz.PassingScore;
                quizToUpdate.UpdatedAt = DateTimeHelper.GetVietnamTime();
                await _quizRepository.UpdateAsync(quizToUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the quiz: " + ex.Message);
            }
        }

        public async Task DeleteQuiz(int quizId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                {
                    throw new Exception("Quiz not found.");
                }
                await _quizRepository.DeleteAsync(quiz);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the quiz: " + ex.Message);
            }
        }
    }
}
