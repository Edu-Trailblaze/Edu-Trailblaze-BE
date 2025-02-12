using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSections()
        {
            try
            {
                var sections = await _sectionService.GetSections();
                return Ok(sections);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-sections-by-conditions")]
        public async Task<IActionResult> GetSectionsByConditions([FromQuery] GetSectionsRequest request)
        {
            try
            {
                var sections = await _sectionService.GetSectionsByConditions(request);
                return Ok(sections);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{sectionId}")]
        public async Task<IActionResult> GetSection(int sectionId)
        {
            try
            {
                var section = await _sectionService.GetSection(sectionId);
                if (section == null)
                {
                    return NotFound();
                }
                return Ok(section);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSection([FromBody] CreateSectionRequest section)
        {
            try
            {
                await _sectionService.AddSection(section);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSection([FromBody] UpdateSectionRequest section)
        {
            try
            {
                await _sectionService.UpdateSection(section);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{sectionId}")]
        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            try
            {
                await _sectionService.DeleteSection(sectionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
