using SmartEmergency.Application.Notifications.DTOs;


namespace SmartEmergency.Application.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateAsync(
            CreateNotificationDto dto,
            CancellationToken cancellationToken = default);

        Task<NotificationListDto> GetUserNotificationsAsync(
            string userId,
            CancellationToken cancellationToken = default);

        Task<List<NotificationDto>> GetByEmergencyRequestAsync(
            long emergencyRequestId,
            CancellationToken cancellationToken = default);

        Task<bool> MarkAsReadAsync(
            long notificationId,
            string userId,
            CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountAsync(
            string userId,
            CancellationToken cancellationToken = default);


        Task NotifyEmergencyRequestSubmittedAsync(
            long emergencyRequestId,
            CancellationToken cancellationToken = default);

        Task NotifyEmergencyRequestStatusChangedAsync(
            long emergencyRequestId,
            CancellationToken cancellationToken = default);


    }
}
