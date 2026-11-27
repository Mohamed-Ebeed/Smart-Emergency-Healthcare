
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Domain.Common;

namespace SmartEmergency.API.Hubs;

[Authorize]
public class EmergencyHub : Hub
{
    private readonly IEmergencyCoordinationService _coordination;
    private readonly IStaffProfileService _staff;

    public EmergencyHub(
        IEmergencyCoordinationService coordination,
        IStaffProfileService staff)
    {
        _coordination = coordination;
        _staff = staff;
    }

    public static string RequestGroup(long requestId) =>
        $"request-{requestId}";

    public static string HospitalGroup(int hospitalId) =>
        $"hospital-{hospitalId}";

    public static string RoleGroup(string role) =>
        $"role-{role}";

    public override async Task OnConnectedAsync()
    {
        foreach (var role in new[] { Roles.Operator, Roles.Admin })
        {
            if (Context.User!.IsInRole(role))
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    RoleGroup(role));
            }
        }

        await base.OnConnectedAsync();
    }

    public async Task JoinRequest(long requestId)
    {
        var userId = UserId();
        var role = Context.User!.FindFirstValue(ClaimTypes.Role);

        try
        {
            await _coordination.EnsureCanViewRequestAsync(
                requestId,
                userId,
                role ?? string.Empty);
        }
        catch (UnauthorizedAccessException)
        {
            throw new HubException(
                "You are not allowed to follow this emergency request.");
        }
        catch (KeyNotFoundException)
        {
            throw new HubException("Emergency request not found.");
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            RequestGroup(requestId));
    }

    public Task LeaveRequest(long requestId) =>
        Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            RequestGroup(requestId));

    public async Task JoinHospital(int hospitalId)
    {
        var user = Context.User!;

        if (!user.IsInRole(Roles.Admin))
        {
            if (!user.IsInRole(Roles.HospitalStaff))
            {
                throw new HubException(
                    "Only hospital staff or admins can follow hospital updates.");
            }

            var userId = UserId();

            var staff = await _staff.GetHospitalStaffByHospitalAsync(
                hospitalId);

            if (!staff.Any(s => s.UserId == userId && s.IsActive))
            {
                throw new HubException(
                    "You can only follow your own hospital.");
            }
        }

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            HospitalGroup(hospitalId));
    }

    private string UserId() =>
        Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new HubException("User ID not found.");
}
