using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Notifications.DTOs;
using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Domain.Entities;
using SmartEmergency.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartEmergency.Application.Notifications.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IAppDbContext _db;
        private readonly INotificationSender _sender;
        private readonly IRealtimeNotifier _realtime;

        public NotificationService(IAppDbContext db, INotificationSender sender, IRealtimeNotifier realtime)
        {
            _db = db;
            _sender = sender;
            _realtime = realtime;
        }

        public async Task<NotificationDto> CreateAsync(
            CreateNotificationDto dto,
            CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            bool hasUser = !string.IsNullOrWhiteSpace(dto.RecipientUserId);
            bool hasContact = dto.EmergencyContactId.HasValue;

            if (hasUser == hasContact)
                throw new ArgumentException(
                    "A notification must have exactly one recipient.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.");

            if (string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException("Message is required.");

            if (!Enum.IsDefined(typeof(NotificationChannel), dto.Channel))
                throw new ArgumentException("Invalid notification channel.");


            if (dto.EmergencyContactId.HasValue)
            {
                bool contactExists = await _db.EmergencyContacts
                    .AnyAsync(
                        c => c.EmergencyContactId == dto.EmergencyContactId.Value,
                        cancellationToken);

                if (!contactExists)
                    throw new KeyNotFoundException("Emergency contact not found.");
            }

            if (dto.EmergencyRequestId.HasValue)
            {
                bool requestExists = await _db.EmergencyRequests
                    .AnyAsync(
                        r => r.EmergencyRequestId == dto.EmergencyRequestId.Value,
                        cancellationToken);

                if (!requestExists)
                    throw new KeyNotFoundException("Emergency request not found.");
            }


            var notification = new Notification
            {
                RecipientUserId = dto.RecipientUserId,
                EmergencyContactId = dto.EmergencyContactId,
                EmergencyRequestId = dto.EmergencyRequestId,
                Title = dto.Title,
                Message = dto.Message,
                Channel = dto.Channel
            };

            _db.Notifications.Add(notification);

            await _db.SaveChangesAsync(cancellationToken);

            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                Title = notification.Title,
                Message = notification.Message,
                Channel = notification.Channel,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                SentAt = notification.SentAt,
                EmergencyRequestId = notification.EmergencyRequestId
            };
        }




        public async Task NotifyEmergencyRequestSubmittedAsync(
            long emergencyRequestId,
            CancellationToken cancellationToken = default)
        {
            var request = await _db.EmergencyRequests
                .Include(r => r.Patient)
                    .ThenInclude(p => p.EmergencyContacts)
                .FirstOrDefaultAsync(
                    r => r.EmergencyRequestId == emergencyRequestId,
                    cancellationToken);

            if (request == null)
                throw new KeyNotFoundException("Emergency request not found.");

            var notifications = new List<Notification>
    {
        new Notification
        {
            RecipientUserId = request.Patient.UserId,
            EmergencyRequestId = request.EmergencyRequestId,
            Title = "Emergency Request Submitted",
            Message = "Your emergency request has been submitted successfully.",
            Channel = NotificationChannel.InApp
        }
    };

            foreach (var contact in request.Patient.EmergencyContacts)
            {
                notifications.Add(new Notification
                {
                    EmergencyContactId = contact.EmergencyContactId,
                    EmergencyRequestId = request.EmergencyRequestId,
                    Title = "Emergency Request Submitted",
                    Message = "An emergency request has been submitted for your emergency contact.",
                    Channel = NotificationChannel.InApp
                });
            }

            _db.Notifications.AddRange(notifications);

            await _db.SaveChangesAsync(cancellationToken);

            await PushLiveAsync(request, notifications, cancellationToken);
        }





        public async Task NotifyEmergencyRequestStatusChangedAsync(
            long emergencyRequestId,
            CancellationToken cancellationToken = default)
        {
            var request = await _db.EmergencyRequests
                .Include(r => r.Patient)
                    .ThenInclude(p => p.EmergencyContacts)
                .FirstOrDefaultAsync(
                    r => r.EmergencyRequestId == emergencyRequestId,
                    cancellationToken);

            if (request == null)
                throw new KeyNotFoundException("Emergency request not found.");

            var notifications = new List<Notification>
    {
        new Notification
        {
            RecipientUserId = request.Patient.UserId,
            EmergencyRequestId = request.EmergencyRequestId,
            Title = "Emergency Request Status Updated",
            Message = $"Your emergency request status is now: {request.CurrentStatus}.",
            Channel = NotificationChannel.InApp
        }
    };

            foreach (var contact in request.Patient.EmergencyContacts)
            {
                notifications.Add(new Notification
                {
                    EmergencyContactId = contact.EmergencyContactId,
                    EmergencyRequestId = request.EmergencyRequestId,
                    Title = "Emergency Request Status Updated",
                    Message = $"The emergency request status has been updated to: {request.CurrentStatus}.",
                    Channel = NotificationChannel.InApp
                });
            }

            _db.Notifications.AddRange(notifications);

            await _db.SaveChangesAsync(cancellationToken);

            await PushLiveAsync(request, notifications, cancellationToken);
        }





        public async Task<List<NotificationDto>> GetByEmergencyRequestAsync(
            long emergencyRequestId,
            CancellationToken cancellationToken = default)
        {
            var notifications = await _db.Notifications
                .AsNoTracking()
                .Where(n => n.EmergencyRequestId == emergencyRequestId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);

            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.Message,
                Channel = n.Channel,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                SentAt = n.SentAt,
                EmergencyRequestId = n.EmergencyRequestId
            }).ToList();
        }




        public async Task<int> GetUnreadCountAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            return await _db.Notifications
                .CountAsync(
                    n => n.RecipientUserId == userId && !n.IsRead,
                    cancellationToken);
        }






        public async Task<NotificationListDto> GetUserNotificationsAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            var query = _db.Notifications
                .AsNoTracking()
                .Where(n => n.RecipientUserId == userId);

            var totalCount = await query.CountAsync(cancellationToken);

            var unreadCount = await query
                .CountAsync(n => !n.IsRead, cancellationToken);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    Channel = n.Channel,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    SentAt = n.SentAt,
                    EmergencyRequestId = n.EmergencyRequestId
                })
                .ToListAsync(cancellationToken);

            return new NotificationListDto
            {
                Notifications = notifications,
                TotalCount = totalCount,
                UnreadCount = unreadCount
            };
        }





        public async Task<bool> MarkAsReadAsync(
            long notificationId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            var notification = await _db.Notifications
                .FirstOrDefaultAsync(
                    n => n.NotificationId == notificationId
                         && n.RecipientUserId == userId,
                    cancellationToken);

            if (notification == null)
                return false;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _db.SaveChangesAsync(cancellationToken);
            }

            return true;
        }

        // Live push: the in-app notifications to their users + the new status to everyone watching
        private async Task PushLiveAsync(EmergencyRequest request, List<Notification> notifications, CancellationToken cancellationToken)
        {
            foreach (var n in notifications.Where(n => n.RecipientUserId != null && n.Channel == NotificationChannel.InApp))
                await _sender.SendAsync(n.RecipientUserId!, n.Title, n.Message, n.Channel, cancellationToken);

            await _realtime.RequestStatusChangedAsync(request.EmergencyRequestId, request.CurrentStatus, cancellationToken);
        }
    }
}