using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class UserProfile : EntityBase<string>
    {
        [Key, ForeignKey("User")]
        public override string Id { get; set; } // Primary and Foreign Key

        public string? Fullname { get; set; }

        public decimal Balance { get; set; } = 0;

        [StringLength(int.MaxValue)]
        public string ProfilePictureUrl { get; set; }


        // Navigation property
        public virtual User User { get; set; }
    }
}
