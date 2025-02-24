using Contracts.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    [Index(nameof(InstructorId), nameof(CourseId), IsUnique = true)]
    public class CourseInstructor : EntityAuditBase<int>
    {


        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [ForeignKey("User")]
        public string InstructorId { get; set; }

        [Required]
        public bool IsPrimaryInstructor { get; set; }

        //


        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual User Instructor { get; set; }
    }
}
