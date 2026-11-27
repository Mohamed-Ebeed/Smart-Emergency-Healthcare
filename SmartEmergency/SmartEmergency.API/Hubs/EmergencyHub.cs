using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Domain.Common;

namespace SmartEmergency.API.Hubs;

// Live updates hub. Client connects to /hubs/emergency?access_token=<jwt>
// Events sent to clients: RequestStatusChanged, HospitalAssignmentChanged, AmbulanceChanged, Notification
[Authorize]
public class EmergencyHub : Hub
{
    private readonly IEmergencyRequestService _requests;
    private readonly IPatientService _patients;
    private readonly IStaffProfileService _staff;

    public EmergencyHub(IEmergencyRequestService requests, IPatientService patients, IStaffProfileService staff)
    {
        _requests = requests;
        _patients = patients;
        _staff = staff;
    }

    public static string RequestGroup(long requestId) => $"request-{requestId}";
    public static string HospitalGroup(int hospitalId) => $"hospital-{hospitalId}";
    public static string RoleGroup(string role) => $"role-{role}";

    // Operators and admins get every update (dashboard)
    public override async Task OnConnectedAsync()
    {
        foreach (var role in new[] { Roles.Operator, Roles.Admin })
            if (Context.User!.IsInRole(role))
                await Groups.AddToGroupAsync(Context.ConnectionId, RoleGroup(role));

        await base.OnConnectedAsync();
    }

    // Follow one request Patient only his own, staff any
    public async Task JoinRequest(long requestId)
    {
        var request = await _requests.GetByIdAsync(requestId);

        if (Context.User!.IsInRole(Roles.Patient))
        {
            var patient = await _patients.GetByUserIdAsync(UserId());
            if (patient is null || patient.PatientId != request.PatientId)
                throw new HubException("You can only follow your own emergency requests.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, RequestGroup(requestId));
    }

    public Task LeaveRequest(long requestId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, RequestGroup(requestId));

    // Hospital staff follow the requests sent to their own hospital
    public async Task JoinHospital(int hospitalId)
    {
        if (!Context.User!.IsInRole(Roles.Admin))
        {
            var staff = await _staff.GetHospitalStaffByHospitalAsync(hospitalId);
            if (!staff.Any(s => s.UserId == UserId() && s.IsActive))
                throw new HubException("You can only follow your own hospital.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, HospitalGroup(hospitalId));
    }

    private string UserId() => Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
}