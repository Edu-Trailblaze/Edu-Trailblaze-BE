using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class Question : EntityAuditBase<int>
    {

        [Required, ForeignKey("Quiz")]
        public int QuizzId { get; set; }

        [Required, StringLength(int.MaxValue)]
        public string QuestionText { get; set; }


        // Navigation properties
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<QuizAnswer> QuizAnswers { get; set; }
    }
}
