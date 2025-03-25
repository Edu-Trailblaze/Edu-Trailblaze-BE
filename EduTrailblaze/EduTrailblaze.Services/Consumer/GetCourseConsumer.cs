using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrailblaze.Services.Consumer
{
    public class GetCourseConsumer : IConsumer<EventBus.Messages.Events.GetCourseRequest>
    {
        private readonly ICourseService _courseService;
        public GetCourseConsumer(ICourseService courseService)
        {
            _courseService = courseService;
        }
        public async Task Consume(ConsumeContext<EventBus.Messages.Events.GetCourseRequest> context)
        {
            Console.WriteLine($"Received request for CourseIds: {context.Message.CourseId}");
            var courses = await _courseService.GetCartCourseInformationAsync(context.Message.CourseId);
            await context.RespondAsync(new EventBus.Messages.Events.CartCourseInformation()
            {
                Id = courses.Id,
                Title = courses.Title,
                Price = courses.Price,
                ImageURL = courses.ImageURL,
                Duration = courses.Duration,
                DifficultyLevel = courses.DifficultyLevel,
                TotalLectures = courses.TotalLectures,
            });
            
    }
    }
}
