using System.ComponentModel.DataAnnotations;
using Contracts.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class Order : EntityBase<int>
    {
        [Required, ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public decimal OrderAmount { get; set; }

        public DateTime OrderDate { get; set; }

        [StringLength(20)]
        public string OrderStatus { get; set; } = "Pending";
        //'Pending', 'Processing', 'Cancelled', 'Completed', 'Refunded', 'Failed'

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}