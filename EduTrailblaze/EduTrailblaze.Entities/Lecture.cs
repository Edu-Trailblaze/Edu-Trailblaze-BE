using EduTrailblaze.API.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class Lecture : EntityAuditBase<int>
    {
        [Required, ForeignKey("Section")]
        public int SectionId { get; set; }

        [Required, StringLength(50)]
        public string Title { get; set; }

        [Required, StringLength(int.MaxValue)]
        public string Content { get; set; }

        [Required, StringLength(int.MaxValue)]
        public string Description { get; set; }

        [Required]
        public int Duration { get; set; } = 0;

        public bool IsDeleted { get; set; } = false;



        // Navigation properties
        public virtual Section Section { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
        public virtual ICollection<UserProgress> UserProgresses { get; set; }
    }
}
