using System.ComponentModel.DataAnnotations;
using Contracts.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class Enrollment : EntityAuditBase<int>
    {


        [Required, ForeignKey("CourseClass")]
        public int CourseClassId { get; set; }

        [Required, ForeignKey("User")]
        public string StudentId { get; set; }

        public decimal ProgressPercentage { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;




        // Navigation properties
        public virtual CourseClass CourseClass { get; set; }
        public virtual User Student { get; set; }
    }
}
