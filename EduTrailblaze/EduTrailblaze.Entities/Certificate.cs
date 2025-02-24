using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class Certificate : EntityAuditBase<int>
    {
        [Required, ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required, StringLength(int.MaxValue)]
        public string CertificateTemplateUrl { get; set; }

        public bool IsDeleted { get; set; } = false;


        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual ICollection<UserCertificate> UserCertificates { get; set; }
    }
}
