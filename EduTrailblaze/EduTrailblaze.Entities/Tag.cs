using System.ComponentModel.DataAnnotations;
using Contracts.Domain;

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
    }
}
