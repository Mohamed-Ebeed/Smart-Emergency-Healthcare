using SmartEmergency.Application.Notifications.DTOs;

namespace SmartEmergency.Application.Notifications.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(CreateNotificationDto notificationDto, CancellationToken cancellationToken = default);
    }
}
