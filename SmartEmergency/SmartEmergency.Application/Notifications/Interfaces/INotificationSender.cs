using SmartEmergency.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEmergency.Application.Notifications.Interfaces
{
    public interface INotificationSender
    {
        Task<bool> SendAsync(
            string recipient,
            string title,
            string message,
            NotificationChannel channel,
            CancellationToken cancellationToken = default);
    }
}
