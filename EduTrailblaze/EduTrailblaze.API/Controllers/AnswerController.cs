using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnswers()
        {
            try
            {
                var answers = await _answerService.GetAnswers();
                if (answers == null)
                {
                    return NotFound();
                }
                return Ok(answers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{answerId}")]
        public async Task<IActionResult> GetAnswer(int answerId)
        {
            try
            {
                var answer = await _answerService.GetAnswer(answerId);
                if (answer == null)
                {
                    return NotFound();
                }
                return Ok(answer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswer([FromBody] CreateAnswerRequest answer)
        {
            try
            {
                await _answerService.AddAnswer(answer);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAnswer([FromBody] UpdateAnswerRequest answer)
        {
            try
            {
                await _answerService.UpdateAnswer(answer);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{answerId}")]
        public async Task<IActionResult> DeleteAnswer(int answerId)
        {
            try
            {
                await _answerService.DeleteAnswer(answerId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
