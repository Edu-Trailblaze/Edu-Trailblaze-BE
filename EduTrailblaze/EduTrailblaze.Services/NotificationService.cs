using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification, int> _notificationRepository;

        public NotificationService(IRepository<Notification, int> notificationRepository)
        {
            _notificationRepository = notificationRepository;
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
    }
}
