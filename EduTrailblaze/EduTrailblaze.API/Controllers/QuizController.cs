using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuiz(int quizId)
        {
            try
            {
                var quiz = await _quizService.GetQuiz(quizId);
                if (quiz == null)
                {
                    return NotFound();
                }
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-quiz-details")]
        public async Task<IActionResult> GetQuizDetails(int lectureId)
        {
            try
            {
                var quiz = await _quizService.GetQuizDetails(lectureId);
                if (quiz == null)
                {
                    return NotFound();
                }
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("create-quiz-details")]
        public async Task<IActionResult> CreateQuizDetails([FromBody] CreateQuizDetails quiz)
        {
            try
            {
                await _quizService.CreateQuizDetails(quiz);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddQuiz([FromBody] CreateQuizRequest quiz)
        {
            try
            {
                await _quizService.AddQuiz(quiz);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateQuiz([FromBody] UpdateQuizRequest quiz)
        {
            try
            {
                await _quizService.UpdateQuiz(quiz);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(int quizId)
        {
            try
            {
                await _quizService.DeleteQuiz(quizId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
