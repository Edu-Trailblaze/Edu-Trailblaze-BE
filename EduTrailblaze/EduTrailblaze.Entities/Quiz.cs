using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class Quiz : EntityAuditBase<int>
    {


        [Required, ForeignKey("Lecture")]
        public int LectureId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required]
        public decimal PassingScore { get; set; }


        // Navigation properties
        public virtual Lecture Lecture { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<QuizHistory> QuizHistories { get; set; }
        public virtual ICollection<UserProgress> UserProgresses { get; set; }
    }
}
