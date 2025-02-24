using Contracts.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    [Index(nameof(DiscountId), nameof(CourseId), IsUnique = true)]
    public class CourseDiscount : EntityBase<int>
    {
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [ForeignKey("Discount")]
        public int DiscountId { get; set; }


        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual Discount Discount { get; set; }
    }
}
