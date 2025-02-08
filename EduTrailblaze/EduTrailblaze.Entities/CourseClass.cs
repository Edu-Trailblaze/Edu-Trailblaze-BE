using EduTrailblaze.API.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class CourseClass : EntityAuditBase<int>
    {
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; }

        [Required, StringLength(int.MaxValue)]
        public string ImageURL { get; set; }

        public string? IntroURL { get; set; }

        [Required, StringLength(int.MaxValue)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Duration { get; set; } = 0;

        [Required]
        [StringLength(50)]
        public string DifficultyLevel { get; set; } // Beginner, Intermediate, Advanced

        [Required, StringLength(int.MaxValue)]
        public string Prerequisites { get; set; }

        [Required]
        public List<string> LearningOutcomes { get; set; }

        [Required]
        public decimal EstimatedCompletionTime { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }


        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<UserProgress> UserProgresses { get; set; }
    }
}