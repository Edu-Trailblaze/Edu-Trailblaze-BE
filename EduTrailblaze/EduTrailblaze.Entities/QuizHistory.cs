using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domain;

namespace EduTrailblaze.Entities
{
    public class QuizHistory : EntityBase<int>
    {

        [Required, ForeignKey("User")]
        public string UserId { get; set; }

        [Required, ForeignKey("Quiz")]
        public int QuizId { get; set; }

        [Required]
        public DateTime AttemptedOn { get; set; }

        [Required]
        public decimal Score { get; set; }

        [Required]
        public bool IsPassed { get; set; }

        [Required]
        public int DurationInSeconds { get; set; }


        // Navigation properties
        public virtual User User { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<QuizAnswer> QuizAnswers { get; set; }
    }
}