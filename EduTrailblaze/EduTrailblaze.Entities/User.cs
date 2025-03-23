using Microsoft.AspNetCore.Identity;

namespace EduTrailblaze.Entities
{
    public class User : IdentityUser
    {
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<QuizHistory> QuizHistories { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<CourseInstructor> CourseInstructors { get; set; }
        public virtual ICollection<UserCertificate> UserCertificates { get; set; }
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
        public virtual ICollection<UserProgress> UserProgresses { get; set; }
        public virtual ICollection<UserCourseCoupon> UserCourseCoupons { get; set; }
    }
}
