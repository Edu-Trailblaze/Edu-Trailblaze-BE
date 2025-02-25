using Contracts.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrailblaze.Entities
{
    [Index(nameof(UserId), nameof(NotificationId), IsUnique = true)]
    public class UserNotification : EntityBase<int>
    {


        [ForeignKey("User")]
        public string UserId { get; set; }

        [ForeignKey("Notification")]
        public int NotificationId { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;


        // Navigation properties
        public virtual User User { get; set; }
        public virtual Notification Notification { get; set; }
    }
}