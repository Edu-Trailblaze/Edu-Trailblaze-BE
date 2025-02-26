using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVideos()
        {
            try
            {
                var videos = await _videoService.GetVideos();
                return Ok(videos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-videos-by-conditions")]
        public async Task<IActionResult> GetVideosByConditions([FromQuery] GetVideosRequest request)
        {
            try
            {
                var videos = await _videoService.GetVideosByConditions(request);
                return Ok(videos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{videoId}")]
        public async Task<IActionResult> GetVideo(int videoId)
        {
            try
            {
                var video = await _videoService.GetVideo(videoId);
                if (video == null)
                {
                    return NotFound();
                }
                return Ok(video);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> AddVideo([FromBody] CreateVideoRequest video)
        //{
        //    try
        //    {
        //        await _videoService.AddVideo(video);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateVideo([FromBody] UpdateVideoRequest video)
        {
            try
            {
                await _videoService.UpdateVideo(video);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{videoId}")]
        public async Task<IActionResult> DeleteVideo(int videoId)
        {
            try
            {
                await _videoService.DeleteVideo(videoId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadVideo([FromForm] UploadVideoRequest video)
        //{
        //    try
        //    {
        //        await _videoService.UploadVideoWithCloudinaryAsync(video);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}
    }
}
