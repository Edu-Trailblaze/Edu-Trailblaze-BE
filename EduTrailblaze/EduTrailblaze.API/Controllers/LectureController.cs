﻿using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureController : ControllerBase
    {
        private readonly ILectureService _lectureService;

        public LectureController(ILectureService lectureService)
        {
            _lectureService = lectureService;
        }

        [HttpGet("{lectureId}")]
        public async Task<IActionResult> GetLecture(int lectureId)
        {
            try
            {
                var lecture = await _lectureService.GetLecture(lectureId);
                if (lecture == null)
                {
                    return NotFound();
                }
                return Ok(lecture);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLectures()
        {
            try
            {
                var lectures = await _lectureService.GetLectures();
                return Ok(lectures);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-lectures-by-conditions")]
        public async Task<IActionResult> GetLecturesByConditions([FromQuery] GetLecturesRequest request)
        {
            try
            {
                var lectures = await _lectureService.GetLecturesByConditions(request);
                return Ok(lectures);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-section-lecture")]
        public async Task<IActionResult> GetSectionLectures([FromQuery] List<int> sectionIds)
        {
            try
            {
                var lectures = await _lectureService.GetSectionLectures(sectionIds);
                return Ok(lectures);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> AddLecture([FromBody] CreateLectureRequest lecture)
        //{
        //    try
        //    {
        //        var res = await _lectureService.AddLecture(lecture);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateLecture([FromForm] UpdateLectureRequest lecture)
        {
            try
            {
                await _lectureService.UpdateLecture(lecture);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{lectureId}")]
        public async Task<IActionResult> DeleteLecture(int lectureId)
        {
            try
            {
                await _lectureService.DeleteLecture(lectureId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpPost("create-lecture")]
        //public async Task<IActionResult> CreateLecture([FromForm] CreateLectureDetails lecture)
        //{
        //    try
        //    {
        //        await _lectureService.CreateLecture(lecture);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        [HttpPost("create-lecture")]
        public async Task<IActionResult> CreateLecture([FromForm] CreateLecture lecture)
        {
            try
            {
                var res = await _lectureService.CreateLecture(lecture);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("create-section-lecture-vip")]
        public async Task<IActionResult> CreateListLectureSection([FromForm] CreateListSectionLectureRequest lectures)
        {
            try
            {

                var res = await _lectureService.CreateListSectionLectures(lectures);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
