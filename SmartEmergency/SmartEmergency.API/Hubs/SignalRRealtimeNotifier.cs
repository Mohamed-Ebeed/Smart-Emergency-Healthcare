using Microsoft.AspNetCore.SignalR;
using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.API.Hubs;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<EmergencyHub> _hub;
    private readonly ILogger<SignalRRealtimeNotifier> _logger;

    public SignalRRealtimeNotifier(
        IHubContext<EmergencyHub> hub,
        ILogger<SignalRRealtimeNotifier> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public Task RequestStatusChangedAsync(
        long requestId,
        RequestStatus status,
        CancellationToken ct = default)
    {
        return SendAsync(
            "RequestStatusChanged",
            new
            {
                requestId,
                status = status.ToString(),
                changedAt = DateTime.UtcNow
            },
            ct,
            EmergencyHub.RequestGroup(requestId),
            StaffGroups());
    }

    public Task HospitalAssignmentChangedAsync(
        int hospitalId,
        long requestId,
        long assignmentId,
        AssignmentStatus status,
        CancellationToken ct = default)
    {
        return SendAsync(
            "HospitalAssignmentChanged",
            new
            {
                hospitalId,
                requestId,
                assignmentId,
                status = status.ToString()
            },
            ct,
            EmergencyHub.HospitalGroup(hospitalId),
            EmergencyHub.RequestGroup(requestId),
            StaffGroups());
    }

    public Task AmbulanceChangedAsync(
        int ambulanceId,
        int hospitalId,
        AmbulanceStatus status,
        decimal? latitude,
        decimal? longitude,
        long? requestId,
        CancellationToken ct = default)
    {
        var groups = new List<string>
        {
            StaffGroups(),
            EmergencyHub.HospitalGroup(hospitalId)
        };

        if (requestId.HasValue)
        {
            groups.Add(
                EmergencyHub.RequestGroup(requestId.Value));
        }

        return SendAsync(
            "AmbulanceChanged",
            new
            {
                ambulanceId,
                hospitalId,
                status = status.ToString(),
                latitude,
                longitude,
                requestId
            },
            ct,
            groups.ToArray());
    }

    private static string StaffGroups()
        => EmergencyHub.RoleGroup(Roles.Operator);

    private async Task SendAsync(
        string eventName,
        object payload,
        CancellationToken ct,
        params string[] groups)
    {
        try
        {
            var allGroups = groups
                .Append(EmergencyHub.RoleGroup(Roles.Admin))
                .Distinct()
                .ToList();

            await _hub.Clients
                .Groups(allGroups)
                .SendAsync(eventName, payload, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Live update {EventName} was not sent.",
                eventName);
        }
    }
}

public class SignalRNotificationSender : INotificationSender
{
    private readonly IHubContext<EmergencyHub> _hub;
    private readonly ILogger<SignalRNotificationSender> _logger;

    public SignalRNotificationSender(
        IHubContext<EmergencyHub> hub,
        ILogger<SignalRNotificationSender> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task<bool> SendAsync(
        string recipient,
        string title,
        string message,
        NotificationChannel channel,
        CancellationToken cancellationToken = default)
    {
        if (channel != NotificationChannel.InApp)
            return false;

        try
        {
            await _hub.Clients
                .User(recipient)
                .SendAsync(
                    "Notification",
                    new { title, message },
                    cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "In-app notification was not sent.");

            return false;
        }
    }
}