using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class QuizAnswer : EntityBase<int>
    {

        [Required, ForeignKey("QuizHistory")]
        public int QuizHistoryId { get; set; }

        [Required, ForeignKey("Question")]
        public int QuestionId { get; set; }

        [Required, StringLength(255)]
        public string UserAnswer { get; set; }

        [Required]
        public bool IsCorrect { get; set; }


        // Navigation properties
        public virtual QuizHistory QuizHistory { get; set; }
        public virtual Question Question { get; set; }
    }
}