using Contracts.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class CourseCoupon : EntityBase<int>
    {


        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [ForeignKey("Coupon")]
        public int CouponId { get; set; }


        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual Coupon Coupon { get; set; }

        public virtual List<UserCourseCoupon> UserCourseCoupons { get; set; }
    }
}