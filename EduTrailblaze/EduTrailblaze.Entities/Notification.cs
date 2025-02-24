using System.ComponentModel.DataAnnotations;
using Contracts.Domain;

namespace EduTrailblaze.Entities
{
    public class Notification : EntityAuditBase<int>
    {

        [Required]
        public string Title { get; set; }

        [Required, StringLength(int.MaxValue)]
        public string Message { get; set; }

        [Required]
        public bool IsGlobal { get; set; }

        [Required]
        public bool IsActive { get; set; }


        // Navigation properties
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
    }
}
