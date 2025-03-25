using EduTrailblaze.Services.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrailblaze.Services.Consumer
{
    public class ReviewConsumer : IConsumer<EventBus.Messages.Events.GetReviewRequest>
    {
        private readonly IReviewService _reviewService;
        public ReviewConsumer(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        public async Task Consume(ConsumeContext<GetReviewRequest> context)
        {
            var review = await _reviewService.GetAverageRatingAndNumberOfRatings(context.Message.CourseId);

            var reviewEvent = review == null
                ? new EventBus.Messages.Events.ReviewInformation ()
                : new EventBus.Messages.Events.ReviewInformation
                {
                    AverageRating = review.AverageRating,
                    TotalRatings = review.TotalRatings

                };

            var response = new EventBus.Messages.Events.GetReviewResponse
            {
                Review = reviewEvent
            };

            await context.RespondAsync(response);
        }
    }
}
