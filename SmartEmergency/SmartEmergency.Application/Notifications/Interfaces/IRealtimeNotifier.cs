using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Notifications.Interfaces;

// Live updates to connected clients
// Application only knows this interface the API decides how to send
public interface IRealtimeNotifier
{
    Task RequestStatusChangedAsync(long requestId, RequestStatus status, CancellationToken ct = default);

    Task HospitalAssignmentChangedAsync(int hospitalId, long requestId, long assignmentId, AssignmentStatus status, CancellationToken ct = default);

    Task AmbulanceChangedAsync(int ambulanceId, AmbulanceStatus status, decimal? latitude, decimal? longitude, long? requestId, CancellationToken ct = default);
}