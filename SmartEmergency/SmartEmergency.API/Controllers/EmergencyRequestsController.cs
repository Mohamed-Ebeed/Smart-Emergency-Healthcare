
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.API.Extensions;
using SmartEmergency.Application.Common.Models;
using SmartEmergency.Application.EmergencyRequests.DTOs;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Application.Patients.DTOs;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.API.Controllers;

[ApiController]
[Route("api/emergency-requests")]
[Authorize]
[TypeFilter(typeof(ApiExceptionFilter))]
[Produces("application/json")]
public class EmergencyRequestsController : ControllerBase
{
    private const string StaffRoles =
        Roles.Operator + "," + Roles.HospitalStaff + "," +
        Roles.Paramedic + "," + Roles.Doctor + "," + Roles.Admin;

    private readonly IEmergencyRequestService _requests;
    private readonly IPatientService _patients;
    private readonly IEmergencyCoordinationService _coordination;

    public EmergencyRequestsController(
        IEmergencyRequestService requests,
        IPatientService patients,
        IEmergencyCoordinationService coordination)
    {
        _requests = requests;
        _patients = patients;
        _coordination = coordination;
    }

    [HttpGet]
    [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
    [ProducesResponseType(typeof(PagedResult<EmergencyRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<EmergencyRequestDto>>> Search(
        [FromQuery] EmergencyRequestFilterDto filter,
        CancellationToken ct)
        => Ok(await _coordination.SearchAsync(filter, ct));

    [HttpPost]
    [Authorize(Roles = Roles.Patient)]
    [ProducesResponseType(typeof(EmergencyRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmergencyRequestDto>> Create(
        [FromBody] CreateEmergencyRequestRequest body,
        CancellationToken ct)
    {
        var patient = await GetCurrentPatientAsync(ct);

        var created = await _requests.CreateAsync(
            new CreateEmergencyRequestDto
            {
                PatientId = patient.PatientId,
                EmergencyTypeId = body.EmergencyTypeId,
                Description = body.Description,
                Latitude = body.Latitude,
                Longitude = body.Longitude,
                AddressText = body.AddressText
            },
            ct);

        return CreatedAtAction(
            nameof(GetById),
            new { requestId = created.EmergencyRequestId },
            created);
    }

    [HttpGet("{requestId:long}")]
    [Authorize(Roles = Roles.Patient + "," + StaffRoles)]
    [ProducesResponseType(typeof(EmergencyRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmergencyRequestDto>> GetById(
        long requestId,
        CancellationToken ct)
    {
        await _coordination.EnsureCanViewRequestAsync(
            requestId,
            GetUserId(),
            GetUserRole(),
            ct);

        var request = await _requests.GetByIdAsync(requestId, ct);

        return Ok(request);
    }

    [HttpGet("mine")]
    [Authorize(Roles = Roles.Patient)]
    [ProducesResponseType(typeof(List<EmergencyRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<EmergencyRequestDto>>> GetMine(
        CancellationToken ct)
    {
        var patient = await GetCurrentPatientAsync(ct);

        return Ok(await _requests.GetByPatientAsync(patient.PatientId, ct));
    }

    [HttpGet("by-patient/{patientId:int}")]
    [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
    [ProducesResponseType(typeof(List<EmergencyRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EmergencyRequestDto>>> GetByPatient(
        int patientId,
        CancellationToken ct)
        => Ok(await _requests.GetByPatientAsync(patientId, ct));

    [HttpPost("{requestId:long}/cancel")]
    [Authorize(Roles = Roles.Patient)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(
        long requestId,
        CancellationToken ct)
    {
        await _requests.CancelAsync(requestId, GetUserId(), ct);

        return NoContent();
    }

    [HttpPost("{requestId:long}/status")]
    [Authorize(Roles = Roles.Operator + "," + Roles.Paramedic + "," + Roles.Admin)]
    [ProducesResponseType(typeof(EmergencyRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmergencyRequestDto>> ChangeStatus(
        long requestId,
        [FromBody] ChangeStatusRequest body,
        CancellationToken ct)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        await _coordination.EnsureCanViewRequestAsync(
            requestId,
            userId,
            userRole,
            ct);

        var current = await _requests.GetByIdAsync(requestId, ct);

        var changed = await _requests.ChangeStatusAsync(
            requestId,
            body.NewStatus,
            userId,
            userRole,
            ct);

        if (!changed)
        {
            throw new InvalidOperationException(
                $"Cannot change status from {current.CurrentStatus} to {body.NewStatus}.");
        }

        return Ok(await _requests.GetByIdAsync(requestId, ct));
    }

    [HttpGet("{requestId:long}/status-history")]
    [Authorize(Roles = Roles.Patient + "," + StaffRoles)]
    [EndpointSummary("Get status history of an emergency request")]
    [EndpointDescription("Every status change with who changed it and when. Access is restricted according to the caller's role and assignment.")]
    [ProducesResponseType(typeof(List<CaseStatusHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<CaseStatusHistoryDto>>> GetStatusHistory(
        long requestId,
        CancellationToken ct)
    {
        return Ok(await _coordination.GetStatusHistoryAsync(
            requestId,
            GetUserId(),
            GetUserRole(),
            ct));
    }

    [HttpPost("{requestId:long}/hospital-assignments")]
    [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
    [EndpointSummary("Send an emergency request to a hospital")]
    [EndpointDescription("Creates a pending hospital assignment and moves the request to SentToHospital.")]
    [ProducesResponseType(typeof(HospitalAssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HospitalAssignmentDto>> SendToHospital(
        long requestId,
        [FromBody] SendToHospitalDto body,
        CancellationToken ct)
        => Ok(await _coordination.SendToHospitalAsync(
            requestId, body, GetUserId(), ct));

    [HttpGet("{requestId:long}/hospital-assignments")]
    [Authorize(Roles = Roles.Patient + "," + StaffRoles)]
    [EndpointSummary("Get the hospitals this request was sent to and their answers")]
    [ProducesResponseType(typeof(List<HospitalAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HospitalAssignmentDto>>> GetHospitalAssignments(
        long requestId,
        CancellationToken ct)
    {
        return Ok(await _coordination.GetHospitalAssignmentsAsync(
            requestId,
            GetUserId(),
            GetUserRole(),
            ct));
    }

    [HttpPost("hospital-assignments/{assignmentId:long}/respond")]
    [Authorize(Roles = Roles.HospitalStaff + "," + Roles.Admin)]
    [EndpointSummary("Hospital accepts or rejects a request")]
    [EndpointDescription("Hospital staff can only answer requests sent to their own hospital.")]
    [ProducesResponseType(typeof(HospitalAssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HospitalAssignmentDto>> RespondToHospitalAssignment(
        long assignmentId,
        [FromBody] HospitalResponseDto body,
        CancellationToken ct)
        => Ok(await _coordination.RespondToHospitalAssignmentAsync(
            assignmentId,
            body,
            GetUserId(),
            User.IsInRole(Roles.Admin),
            ct));

    [HttpPost("{requestId:long}/ambulance-assignments")]
    [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
    [EndpointSummary("Assign an ambulance and a paramedic to a request")]
    [ProducesResponseType(typeof(AmbulanceAssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AmbulanceAssignmentDto>> AssignAmbulance(
        long requestId,
        [FromBody] AssignAmbulanceDto body,
        CancellationToken ct)
        => Ok(await _coordination.AssignAmbulanceAsync(
            requestId, body, GetUserId(), ct));

    [HttpGet("{requestId:long}/next-hospital")]
    [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
    [EndpointSummary("Suggest the next hospital to try")]
    [ProducesResponseType(typeof(NextHospitalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NextHospitalDto>> GetNextHospital(
        long requestId,
        CancellationToken ct)
        => Ok(await _coordination.GetNextHospitalAsync(requestId, ct));

    [HttpPost("{requestId:long}/hospital-assignments/next")]
    [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
    [EndpointSummary("Send the request to the next suitable hospital")]
    [ProducesResponseType(typeof(HospitalAssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HospitalAssignmentDto>> SendToNextHospital(
        long requestId,
        CancellationToken ct)
        => Ok(await _coordination.SendToNextHospitalAsync(
            requestId, GetUserId(), ct));

    [HttpGet("{requestId:long}/ambulance-assignments")]
    [Authorize(Roles = Roles.Patient + "," + StaffRoles)]
    [EndpointSummary("Get the ambulance assignments of a request")]
    [ProducesResponseType(typeof(List<AmbulanceAssignmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AmbulanceAssignmentDto>>> GetAmbulanceAssignments(
        long requestId,
        CancellationToken ct)
    {
        return Ok(await _coordination.GetAmbulanceAssignmentsAsync(
            requestId,
            GetUserId(),
            GetUserRole(),
            ct));
    }

    [HttpGet("ambulance-assignments/{assignmentId:long}")]
    [Authorize(Roles = StaffRoles)]
    [EndpointSummary("Get one ambulance assignment")]
    [ProducesResponseType(typeof(AmbulanceAssignmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AmbulanceAssignmentDto>> GetAmbulanceAssignment(
        long assignmentId,
        CancellationToken ct)
        => Ok(await _coordination.GetAmbulanceAssignmentAsync(
            assignmentId,
            GetUserId(),
            GetUserRole(),
            ct));

    [HttpGet("ambulance-assignments/mine")]
    [Authorize(Roles = Roles.Paramedic)]
    [EndpointSummary("Get my assignments (logged in paramedic)")]
    [ProducesResponseType(typeof(List<AmbulanceAssignmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AmbulanceAssignmentDto>>> GetMyAmbulanceAssignments(
        CancellationToken ct)
        => Ok(await _coordination.GetMyAmbulanceAssignmentsAsync(
            GetUserId(), ct));

    // ================= Helpers =================

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException(
            "User id was not found in the access token.");

    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role)
        ?? throw new UnauthorizedAccessException(
            "User role was not found in the access token.");

    private async Task<PatientDto> GetCurrentPatientAsync(
        CancellationToken ct) =>
        await _patients.GetByUserIdAsync(GetUserId(), ct)
        ?? throw new UnauthorizedAccessException(
            "No patient account is linked to the current user.");
}

// ================= Request models (Swagger) =================

public class CreateEmergencyRequestRequest
{
    [Range(1, int.MaxValue)]
    public int EmergencyTypeId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(typeof(decimal), "-90", "90")]
    public decimal? Latitude { get; set; }

    [Range(typeof(decimal), "-180", "180")]
    public decimal? Longitude { get; set; }

    [StringLength(500)]
    public string? AddressText { get; set; }
}

public class ChangeStatusRequest
{
    [Required, EnumDataType(typeof(RequestStatus))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RequestStatus NewStatus { get; set; }
}
