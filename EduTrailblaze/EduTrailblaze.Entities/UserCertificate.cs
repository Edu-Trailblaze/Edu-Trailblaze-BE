using Contracts.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    public class UserCertificate : EntityBase<int>
    {

        [Required, ForeignKey("User")]
        public string UserId { get; set; }

        [Required, ForeignKey("Certificate")]
        public int CertificateId { get; set; }

        public DateTimeOffset IssuedAt { get; set; }

        [Required]
        public string CertificateUrl { get; set; }


        // Navigation properties
        public virtual User User { get; set; }
        public virtual Certificate Certificate { get; set; }
    }
}
