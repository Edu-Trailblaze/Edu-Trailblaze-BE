using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class UserTag : EntityBase<int>
    {
        [Required, ForeignKey("User")]
        public string UserId { get; set; }

        [Required, ForeignKey("Tag")]
        public int TagId { get; set; }


        // Navigation properties
        public virtual User User { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
