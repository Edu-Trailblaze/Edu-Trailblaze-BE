using Contracts.Domain;
using System.ComponentModel.DataAnnotations;

namespace EduTrailblaze.Entities
{
    public class Tag : EntityAuditBase<int>
    {

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }


        // Navigation property
        public virtual ICollection<CourseTag> CourseTags { get; set; }
        public virtual ICollection<UserTag> UserTags { get; set; }
    }
}
