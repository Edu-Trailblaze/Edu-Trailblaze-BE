using EduTrailblaze.API.Domain.Interfaces;
using EduTrailblaze.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EduTrailblaze.Repositories
{
    public class EduTrailblazeDbContext : IdentityDbContext<User>
    {
        public EduTrailblazeDbContext(DbContextOptions<EduTrailblazeDbContext> options)
            : base(options)
        {
        }

        public EduTrailblazeDbContext()
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        //    IConfigurationRoot configurationRoot = builder.Build();
        //    optionsBuilder.UseSqlServer(configurationRoot.GetConnectionString("DefaultConnection"));
        //}

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseClass> CourseClasses { get; set; }
        public virtual DbSet<CourseCoupon> CourseCoupons { get; set; }
        public virtual DbSet<CourseDiscount> CourseDiscounts { get; set; }
        public virtual DbSet<CourseInstructor> CourseInstructors { get; set; }
        public virtual DbSet<CourseLanguage> CourseLanguages { get; set; }
        public virtual DbSet<CourseTag> CourseTags { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Lecture> Lectures { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<QuizAnswer> QuizAnswers { get; set; }
        public virtual DbSet<QuizHistory> QuizHistories { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<UserCertificate> UserCertificates { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }
        public virtual DbSet<UserProgress> UserProgresses { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var modifiedEntries = ChangeTracker
                .Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted);

            foreach (var entry in modifiedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        //auto add created date
                        if (entry.Entity is IDateTracking addedEntity)
                        {
                            entry.State = EntityState.Added;
                        }
                        break;
                    case EntityState.Modified:
                        Entry(entry.Entity).Property("Id").IsModified = false; //cannot change Id
                        if (entry.Entity is IDateTracking modifiedEntities)
                        {
                            entry.State = EntityState.Modified;
                        }
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserCertificate>()
                .Property(n => n.IssuedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<CourseInstructor>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Tag>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Tag>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<UserProgress>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<UserProgress>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Quiz>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Quiz>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Question>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Question>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Review>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Review>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Review>()
                .Property(n => n.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<Certificate>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Certificate>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Certificate>()
                .Property(n => n.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<Notification>()
                .Property(n => n.IsActive)
                .HasDefaultValue(true);

            builder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Course>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Course>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Course>()
                .Property(n => n.IsPublished)
                .HasDefaultValue(false);

            builder.Entity<Course>()
                .Property(n => n.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<Voucher>()
                .Property(n => n.IsUsed)
                .HasDefaultValue(false);

            builder.Entity<Video>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Video>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Video>()
                .Property(n => n.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<Lecture>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Lecture>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Lecture>()
                .Property(n => n.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<Enrollment>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Enrollment>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Enrollment>()
                .Property(n => n.ProgressPercentage)
                .HasDefaultValue(0m);
            builder.Entity<Enrollment>()
                .Property(n => n.IsCompleted)
                .HasDefaultValue(false);

            builder.Entity<UserNotification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Entity<Discount>()
                .Property(n => n.IsActive)
                .HasDefaultValue(true);

            builder.Entity<Cart>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Cart>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Cart>()
                .Property(n => n.IsActive)
                .HasDefaultValue(true);

            builder.Entity<Coupon>()
                .Property(n => n.IsActive)
                .HasDefaultValue(true);

            builder.Entity<News>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<News>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<News>()
                .Property(n => n.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<UserCourseCoupon>()
                .Property(n => n.UsageDate)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<UserCourseCoupon>()
                .Property(n => n.IsRedeemed)
                .HasDefaultValue(false);

            builder.Entity<Order>()
                .Property(n => n.OrderDate)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Order>()
                .Property(n => n.OrderStatus)
                .HasDefaultValue("Pending");

            builder.Entity<Payment>()
                .Property(n => n.PaymentDate)
                .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

            builder.Entity<Payment>()
                .Property(n => n.PaymentStatus)
                .HasDefaultValue("Processing");

            builder.Entity<QuizAnswer>()
                .HasOne(q => q.QuizHistory)
                .WithMany(qh => qh.QuizAnswers)
                .HasForeignKey(q => q.QuizHistoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserProfile>()
                .Property(n => n.Balance)
                .HasDefaultValue(0m);

            builder.Entity<UserProfile>()
                .Property(n => n.ProfilePictureUrl)
                .HasDefaultValue("https://firebasestorage.googleapis.com/v0/b/storage-8b808.appspot.com/o/OIP.jpeg?alt=media&token=60195a0a-2fd6-4c66-9e3a-0f7f80eb8473");

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "R001",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole
                {
                    Id = "R002",
                    Name = "Instructor",
                    NormalizedName = "INSTRUCTOR",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole
                {
                    Id = "R003",
                    Name = "Student",
                    NormalizedName = "STUDENT",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );
        }
    }
}
