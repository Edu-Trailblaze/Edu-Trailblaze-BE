using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface INotificationService
    {
        Task<Notification?> GetNotification(int notificationId);

        Task<IEnumerable<Notification>> GetNotifications();

        Task AddNotification(Notification notification);

        Task UpdateNotification(Notification notification);

        Task DeleteNotification(Notification notification);

        Task AddNotification(CreateNotificationRequest notification);

        Task UpdateNotification(UpdateNotificationRequest notification);

        Task DeleteNotification(int notification);

        Task NotifyRecentActitity(string title, string message, string userId);
    }
}
