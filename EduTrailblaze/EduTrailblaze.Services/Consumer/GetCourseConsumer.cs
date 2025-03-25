using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrailblaze.Services.Consumer
{
    public class GetCourseConsumer : IConsumer<EventBus.Messages.Events.GetCourseRequest>, IConsumer<EventBus.Messages.Events.GetInstructorRequest>, IConsumer<EventBus.Messages.Events.GetDiscountRequest>,
     IConsumer<EventBus.Messages.Events.GetCouponRequest>
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

        public async Task Consume(ConsumeContext<GetInstructorRequest> context)
        {
            var instructorList = await _courseService.InstructorInformation(context.Message.CourseId);

            var instructors = instructorList?.Select(dto => new EventBus.Messages.Events.InstructorInformation
            {
                Id = dto.Id,
                UserName = dto.UserName,
                Fullname = dto.Fullname,
                ProfilePictureUrl =     dto.ProfilePictureUrl,
                Email = dto.Email
            }).ToList() ?? new List<EventBus.Messages.Events.InstructorInformation>();

            var response = new EventBus.Messages.Events.GetInstructorResponse
            {
                Instructors = instructors
            };

            await context.RespondAsync(response);
        }

        public async Task Consume(ConsumeContext<GetDiscountRequest> context)
        {
            try
            {
                Console.WriteLine($"[GetDiscountConsumer] Received request for CourseId: {context.Message.CourseId}");

                var discount = await _courseService.DiscountInformationResponse(context.Message.CourseId);

                var discountEvent = discount == null
                    ? new EventBus.Messages.Events.DiscountInformation { DiscountType = "None", DiscountValue = 0 }
                    : new EventBus.Messages.Events.DiscountInformation
                    {
                        DiscountType = discount.DiscountType,
                        DiscountValue = discount.DiscountValue
                    };

                var response = new EventBus.Messages.Events.GetDiscountResponse
                {
                    Discount = discountEvent
                };

                await context.RespondAsync(response);

                Console.WriteLine("[GetDiscountConsumer] Response sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetDiscountConsumer] Error: {ex.Message}");
                throw;
            }
        }

        public async Task Consume(ConsumeContext<GetCouponRequest> context)
        {
            var coupon = await _courseService.CouponInformation(context.Message.CourseId,context.Message.UserId);

            var couponEvent = coupon == null
                ? new EventBus.Messages.Events.CouponInformation { DiscountType = "None", DiscountValue = 0 }
                : new EventBus.Messages.Events.CouponInformation
                {
                    DiscountType = coupon.DiscountType,
                    DiscountValue =coupon.DiscountValue,
                    Code = coupon.Code
                   
                };

            var response = new EventBus.Messages.Events.GetCouponResponse
            {
                Coupon = couponEvent
            };

            await context.RespondAsync(response);
        }
    }
}
