using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class Video : EntityAuditBase<int>
    {

        [Required, ForeignKey("Lecture")]
        public int LectureId { get; set; }

        [Required, StringLength(50)]
        public string Title { get; set; }

        [StringLength(int.MaxValue)]
        public string VideoUrl { get; set; }

        [StringLength(int.MaxValue)]
        public string? Transcript { get; set; }

        public TimeSpan Duration { get; set; } = new TimeSpan(0, 0, 0);

        [Required]
        public bool IsDeleted { get; set; } = false;


        // Navigation properties
        public virtual Lecture Lecture { get; set; }
    }
}
