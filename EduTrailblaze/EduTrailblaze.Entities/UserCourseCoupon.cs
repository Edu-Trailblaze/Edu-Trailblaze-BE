using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class UserCourseCoupon : EntityBase<int>
    {


        [Required, ForeignKey("User")]
        public string UserId { get; set; }

        [Required, ForeignKey("CourseCoupon")]
        public int CourseCouponId { get; set; }

        public DateTime UsageDate { get; set; }

        public bool IsRedeemed { get; set; }


        // Navigation properties
        public virtual User User { get; set; }
        public virtual CourseCoupon CourseCoupon { get; set; }
    }
}
