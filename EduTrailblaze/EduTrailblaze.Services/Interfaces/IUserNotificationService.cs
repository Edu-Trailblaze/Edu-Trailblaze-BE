using EduTrailblaze.Entities;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IUserNotificationService
    {
        Task<UserNotification?> GetUserNotification(int userNotificationId);

        Task<IEnumerable<UserNotification>> GetUserNotifications();

        Task AddUserNotification(UserNotification userNotification);

        Task UpdateUserNotification(UserNotification userNotification);

        Task DeleteUserNotification(UserNotification userNotification);

        Task MarkAsReadAsync(int notificationId);
    }
}
