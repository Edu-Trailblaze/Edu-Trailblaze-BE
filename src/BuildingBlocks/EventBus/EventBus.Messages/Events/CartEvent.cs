using EventBus.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public record CartEvent() : IntegrationBaseEvent,ICartEvent
    {
        public int CourseId { get ; set ; }

    }
    public class GetCourseRequest
    {
        public int CourseId { get; set; }
    }
    public class GetInstructorRequest
    {
        public int CourseId { get; set; }
    }
    public class GetDiscountRequest
    {
        public int CourseId { get; set; }
    }    
    public class GetReviewRequest
    {
        public int CourseId { get; set; }
    }
    public class GetCouponRequest
    {
        public int CourseId { get; set; }
        public string UserId { get; set; }
    }
    public class CartItemInformation
    {
        public CartCourseInformation CartCourseInformation { get; set; }
        public List<InstructorInformation> InstructorInformation { get; set; }
        public EventBus.Messages.Events.ReviewInformation CourseReviewInformation { get; set; }
        public DiscountInformation? DiscountInformation { get; set; }
        public CouponInformation? CouponInformation { get; set; }
        public decimal TotalCoursePrice { get; set; }
    }
    public class ReviewInformation
    {
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
    public class CartCourseInformation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string ImageURL { get; set; }
        public int Duration { get; set; }
        public string DifficultyLevel { get; set; }
        public int TotalLectures { get; set; }
    }
    public class InstructorInformation
    {
        public string Id { get; set; }
        public string Fullname { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
    public class GetInstructorResponse
    {
        public List<InstructorInformation> Instructors { get; set; }
    }
    public class GetDiscountResponse
    {
        public DiscountInformation Discount { get; set; }
    }
    public class GetCouponResponse
    {
        public CouponInformation Coupon { get; set; }
    }
    public class GetReviewResponse
    {
        public EventBus.Messages.Events.ReviewInformation Review { get; set; }
    }
    public class CouponInformation
    {
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal CalculatedDiscount { get; private set; }
        public decimal CalculatedPrice { get; private set; }

        public void CalculateDiscountAndPrice(decimal price)
        {
            CalculatedDiscount = DiscountType == "Percentage" ? price * DiscountValue / 100 : DiscountValue;
            CalculatedPrice = price - CalculatedDiscount;
        }
    }
    public class DiscountInformation
    {
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal CalculatedDiscount { get; private set; }
        public decimal CalculatedPrice { get; private set; }

        public void CalculateDiscountAndPrice(decimal price)
        {
            CalculatedDiscount = DiscountType == "Percentage" ? price * DiscountValue / 100 : DiscountValue;
            CalculatedPrice = price - CalculatedDiscount;
        }
    }
}
