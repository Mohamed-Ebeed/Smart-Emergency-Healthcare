using SmartEmergency.Application.Notifications.Interfaces;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Notifications.Services;

// Default that does nothing Used by projects with no SignalR
// The API replaces these with the SignalR versions
public class NoRealtimeNotifier : IRealtimeNotifier
{
    public Task RequestStatusChangedAsync(long requestId, RequestStatus status, CancellationToken ct = default) => Task.CompletedTask;

    public Task HospitalAssignmentChangedAsync(int hospitalId, long requestId, long assignmentId, AssignmentStatus status, CancellationToken ct = default) => Task.CompletedTask;

    public Task AmbulanceChangedAsync(int ambulanceId, AmbulanceStatus status, decimal? latitude, decimal? longitude, long? requestId, CancellationToken ct = default) => Task.CompletedTask;
}

public class NoNotificationSender : INotificationSender
{
    public Task<bool> SendAsync(string recipient, string title, string message, NotificationChannel channel, CancellationToken cancellationToken = default)
        => Task.FromResult(false);
}