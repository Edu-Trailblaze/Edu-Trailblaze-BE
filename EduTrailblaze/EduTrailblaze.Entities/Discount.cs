using Contracts.Domain;
using System.ComponentModel.DataAnnotations;

namespace EduTrailblaze.Entities
{
    public class Discount : EntityBase<int>
    {

        [Required, StringLength(50)]
        public string DiscountType { get; set; } //'Percentage', 'Value'

        [Required]
        public decimal DiscountValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? UsageCount { get; set; }

        public int? MaxUsage { get; set; }

        public bool IsActive { get; set; } = true;


        // Navigation properties
        public virtual ICollection<CourseDiscount> CourseDiscounts { get; set; }
    }
}
