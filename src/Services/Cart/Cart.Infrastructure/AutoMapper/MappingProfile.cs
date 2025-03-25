using AutoMapper;
using Cart.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EventBus.Messages.Events.CartCourseInformation, CartCourseInformation>();

           
            CreateMap<EventBus.Messages.Events.DiscountInformation, DiscountInformation>();
            CreateMap<EventBus.Messages.Events.CouponInformation, CouponInformation>();

            CreateMap<EventBus.Messages.Events.ReviewInformation, ReviewInformation>();
            CreateMap<EventBus.Messages.Events.InstructorInformation, InstructorInformation>();
            CreateMap<EventBus.Messages.Events.CartItemInformation, CartItemInformation>();

        }
    }

}
