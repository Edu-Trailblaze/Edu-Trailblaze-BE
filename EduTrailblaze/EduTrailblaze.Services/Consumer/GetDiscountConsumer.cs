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
    public class GetDiscountConsumer : IConsumer<EventBus.Messages.Events.GetDiscountRequest>
    {
        private readonly IDiscountService _discountService;
        public GetDiscountConsumer(IDiscountService _discountService)
        {
            _discountService = _discountService;
        }
        public async Task Consume(ConsumeContext<GetDiscountRequest> context)
        {
           throw new NotImplementedException();
        }
    }
}
