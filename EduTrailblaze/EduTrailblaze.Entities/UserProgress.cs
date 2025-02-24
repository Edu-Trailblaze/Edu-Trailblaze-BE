using System.ComponentModel.DataAnnotations;
using Contracts.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class UserProgress : EntityAuditBase<int>
    {
        [Required, ForeignKey("User")]
        public string UserId { get; set; }

        [Required, ForeignKey("CourseClass")]
        public int CourseClassId { get; set; }

        [ForeignKey("Section")]
        public int? SectionId { get; set; }

        [ForeignKey("Lecture")]
        public int? LectureId { get; set; }

        [ForeignKey("Quiz")]
        public int? QuizId { get; set; }

        [Required, StringLength(50)]
        public string ProgressType { get; set; }
        //'Course', 'Section', 'Lecture', 'Quiz'

        public decimal ProgressPercentage { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;

        [Required]
        public DateTimeOffset LastAccessed { get; set; }



        // Navigation properties
        public virtual User User { get; set; }
        public virtual CourseClass CourseClass { get; set; }
        public virtual Section Section { get; set; }
        public virtual Lecture Lecture { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
