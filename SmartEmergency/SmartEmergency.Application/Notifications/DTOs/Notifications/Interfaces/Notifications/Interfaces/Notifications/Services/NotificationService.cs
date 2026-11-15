using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Notifications.DTOs;
using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Application.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IAppDbContext _context;

        public NotificationService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
        {
            bool hasUser = !string.IsNullOrEmpty(dto.RecipientUserId);
            bool hasContact = dto.EmergencyContactId.HasValue && dto.EmergencyContactId.Value > 0;

            if ((hasUser && hasContact) || (!hasUser && !hasContact))
            {
                throw new ArgumentException("Each notification must have exactly one recipient: either RecipientUserId or EmergencyContactId.");
            }

            var notification = new Notification
            {
                RecipientUserId = dto.RecipientUserId,
                EmergencyContactId = dto.EmergencyContactId,
                EmergencyRequestId = dto.EmergencyRequestId,
                Title = dto.Title,
                Message = dto.Message,
                Channel = dto.Channel,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return notification.NotificationId;
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetNotificationsByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.notifications
                .Where(n => n.RecipientUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    NotificationId = n.NotificationId,
                    RecipientUserId = n.RecipientUserId,
                    EmergencyContactId = n.EmergencyContactId,
                    EmergencyRequestId = n.EmergencyRequestId,
                    Title = n.Title,
                    Message = n.Message,
                    Channel = n.Channel,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    SentAt = n.SentAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetNotificationsByEmergencyRequestAsync(long emergencyRequestId, CancellationToken cancellationToken = default)
        {
            return await _context.notifications
                .Where(n => n.EmergencyRequestId == emergencyRequestId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDto
                {
                    NotificationId = n.NotificationId,
                    RecipientUserId = n.RecipientUserId,
                    EmergencyContactId = n.EmergencyContactId,
                    EmergencyRequestId = n.EmergencyRequestId,
                    Title = n.Title,
                    Message = n.Message,
                    Channel = n.Channel,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    SentAt = n.SentAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task MarkAsReadAsync(long notificationId, CancellationToken cancellationToken = default)
        {
            var notification = await _context.notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId, cancellationToken);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<int> GetUnreadCountAsync(string? userId = null, long? emergencyContactId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.notifications.Where(n => !n.IsRead);

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(n => n.RecipientUserId == userId);
            }
            else if (emergencyContactId.HasValue)
            {
                query = query.Where(n => n.EmergencyContactId == emergencyContactId.Value);
            }

            return await query.CountAsync(cancellationToken);
        }
    }
}
