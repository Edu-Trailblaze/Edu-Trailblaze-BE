using Contracts.Domain;
using System.ComponentModel.DataAnnotations;

namespace EduTrailblaze.Entities
{
    public class Language : EntityBase<int>
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }


        // Navigation properties
        public virtual ICollection<CourseLanguage> CourseLanguages { get; set; }
    }
}
