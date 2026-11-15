using SmartEmergency.Application.Notifications.DTOs;

namespace SmartEmergency.Application.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<long> CreateNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);
        Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<NotificationResponseDto>> GetNotificationsByEmergencyRequestAsync(long emergencyRequestId, CancellationToken cancellationToken = default);
        Task MarkAsReadAsync(long notificationId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(string? userId = null, long? emergencyContactId = null, CancellationToken cancellationToken = default);
    }
}
