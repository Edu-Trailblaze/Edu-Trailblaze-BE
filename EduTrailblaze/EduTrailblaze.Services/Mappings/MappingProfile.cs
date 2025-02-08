using AutoMapper;
using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CoursesResponse>();
            CreateMap<Course, CourseDetails>();
            CreateMap<Course, CourseDTO>();
            CreateMap<CreateCourseRequest, Course>();
            CreateMap<CourseDTO, CoursesResponse>();
            CreateMap<Course, CartCourseInformation>();

            CreateMap<Discount, DiscountInformationResponse>();
            CreateMap<Discount, DiscountInformation>();

            CreateMap<User, InstructorInformation>();

            CreateMap<Coupon, CouponInformation>();

            CreateMap<Order, OrderDTO>();

            CreateMap<Language, SupportedLanguage>();

            CreateMap<Section, SectionDetails>();
        }
    }
}