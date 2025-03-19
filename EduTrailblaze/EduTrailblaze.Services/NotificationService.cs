using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Hubs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace EduTrailblaze.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification, int> _notificationRepository;
        private readonly IRepository<UserProfile, string> _userProfileRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IRepository<Notification, int> notificationRepository, IHubContext<NotificationHub> hubContext, IRepository<UserProfile, string> userProfileRepository)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<Notification?> GetNotification(int notificationId)
        {
            try
            {
                return await _notificationRepository.GetByIdAsync(notificationId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the notification: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            try
            {
                return await _notificationRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the notification: " + ex.Message);
            }
        }

        public async Task AddNotification(Notification notification)
        {
            try
            {
                await _notificationRepository.AddAsync(notification);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the notification: " + ex.Message);
            }
        }

        public async Task UpdateNotification(Notification notification)
        {
            try
            {
                await _notificationRepository.UpdateAsync(notification);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the notification: " + ex.Message);
            }
        }

        public async Task DeleteNotification(Notification notification)
        {
            try
            {
                await _notificationRepository.DeleteAsync(notification);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the notification: " + ex.Message);
            }
        }

        public async Task AddNotification(CreateNotificationRequest notification)
        {
            try
            {
                var notificationEntity = new Notification
                {
                    Title = notification.Title,
                    Message = notification.Message,
                    IsGlobal = notification.IsGlobal,
                };
                await _notificationRepository.AddAsync(notificationEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the notification: " + ex.Message);
            }
        }

        public async Task UpdateNotification(UpdateNotificationRequest notification)
        {
            try
            {
                var notificationEntity = await _notificationRepository.GetByIdAsync(notification.NotificationId);
                if (notificationEntity == null)
                {
                    throw new Exception("Notification not found.");
                }
                notificationEntity.Title = notification.Title;
                notificationEntity.Message = notification.Message;
                notificationEntity.IsGlobal = notification.IsGlobal;
                notificationEntity.IsActive = notification.IsActive;

                await _notificationRepository.UpdateAsync(notificationEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the notification: " + ex.Message);
            }
        }

        public async Task DeleteNotification(int notificationId)
        {
            try
            {
                var notificationEntity = await _notificationRepository.GetByIdAsync(notificationId);
                if (notificationEntity == null)
                {
                    throw new Exception("Notification not found.");
                }

                notificationEntity.IsActive = false;

                await _notificationRepository.UpdateAsync(notificationEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the notification: " + ex.Message);
            }
        }

        public async Task NotifyRecentActitity(string title, string message, string userId)
        {
            try
            {
                var notification = new Notification
                {
                    Title = title,
                    Message = message,
                    UserNotifications = new List<UserNotification>
                    {
                        new UserNotification
                        {
                            UserId = userId,
                        }
                    }
                };

                await _notificationRepository.AddAsync(notification);

                var user = await _userProfileRepository.GetByIdAsync(userId);

                if (user != null)
                {
                    await _hubContext.Clients.User(user.Id).SendAsync("ReceiveNotification", new
                    {
                        Message = message,
                        ImageUrl = user.ProfilePictureUrl,
                        CreatedAt = notification.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the notification: " + ex.Message);
            }
        }
    }
}
