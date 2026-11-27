using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Notifications.Interfaces;

public interface IRealtimeNotifier
{
    Task RequestStatusChangedAsync(
        long requestId,
        RequestStatus status,
        CancellationToken ct = default);

    Task HospitalAssignmentChangedAsync(
        int hospitalId,
        long requestId,
        long assignmentId,
        AssignmentStatus status,
        CancellationToken ct = default);

    Task AmbulanceChangedAsync(
        int ambulanceId,
        int hospitalId,
        AmbulanceStatus status,
        decimal? latitude,
        decimal? longitude,
        long? requestId,
        CancellationToken ct = default);
}