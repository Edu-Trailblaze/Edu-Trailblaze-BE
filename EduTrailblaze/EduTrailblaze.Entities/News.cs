using Contracts.Domain;
using System.ComponentModel.DataAnnotations;

namespace EduTrailblaze.Entities
{
    public class News : EntityAuditBase<int>
    {
        [Required, StringLength(255)]
        public string Title { get; set; }

        [Required, StringLength(int.MaxValue)]
        public string Content { get; set; }

        [StringLength(int.MaxValue)]
        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}