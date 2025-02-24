using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class Cart : EntityAuditBase<int>
    {
        [Required, ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;


        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
