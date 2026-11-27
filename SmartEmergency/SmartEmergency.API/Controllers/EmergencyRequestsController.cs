using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.API.Extensions;
using SmartEmergency.Application.EmergencyRequests.DTOs;
using SmartEmergency.Application.EmergencyRequests.Interfaces;
using SmartEmergency.Application.Patients.DTOs;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.API.Controllers;

/// <summary>
/// Emergency requests API. مفيش Business Logic هنا: بنجيب الـ patientId من المستخدم المسجّل،
/// وبننده الـ Services الموجودة، والـ ApiExceptionFilter بيحوّل الاستثناءات لـ HTTP status.
/// </summary>
[ApiController]
[Route("api/emergency-requests")]
[Authorize]
[TypeFilter(typeof(ApiExceptionFilter))]
[Produces("application/json")]
public class EmergencyRequestsController : ControllerBase
{
    private const string StaffRoles =
        Roles.Operator + "," + Roles.HospitalStaff + "," + Roles.Paramedic + "," + Roles.Doctor + "," + Roles.Admin;

    private readonly IEmergencyRequestService _requests;
    private readonly IPatientService _patients;

    public EmergencyRequestsController(IEmergencyRequestService requests, IPatientService patients)
    {
        _requests = requests;
        _patients = patients;
    }

    /// <summary>Create a new emergency request for the logged-in patient.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Patient)]
    [ProducesResponseType(typeof(EmergencyRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmergencyRequestDto>> Create(
        [FromBody] CreateEmergencyRequestRequest body, CancellationToken ct)
    {
        // الـ patientId من المستخدم المسجّل، مش من الـ body ولا الـ URL
        var patient = await GetCurrentPatientAsync(ct);

        var created = await _requests.CreateAsync(new CreateEmergencyRequestDto
        {
            PatientId = patient.PatientId,
            EmergencyTypeId = body.EmergencyTypeId,
            Description = body.Description,
            Latitude = body.Latitude,
            Longitude = body.Longitude,
            AddressText = body.AddressText
        }, ct);

        return CreatedAtAction(nameof(GetById), new { requestId = created.EmergencyRequestId }, created);
    }

    /// <summary>Get an emergency request by id. Patients can only see their own requests.</summary>
    [HttpGet("{requestId:long}")]
    [Authorize(Roles = Roles.Patient + "," + StaffRoles)]
    [ProducesResponseType(typeof(EmergencyRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmergencyRequestDto>> GetById(long requestId, CancellationToken ct)
    {
        var request = await _requests.GetByIdAsync(requestId, ct);

        if (User.IsInRole(Roles.Patient))
        {
            var patient = await GetCurrentPatientAsync(ct);
            if (patient.PatientId != request.PatientId)
                throw new UnauthorizedAccessException("You can only view your own emergency requests.");
        }

        return Ok(request);
    }

    /// <summary>Get the logged-in patient's emergency requests.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = Roles.Patient)]
    [ProducesResponseType(typeof(List<EmergencyRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<EmergencyRequestDto>>> GetMine(CancellationToken ct)
    {
        var patient = await GetCurrentPatientAsync(ct);
        return Ok(await _requests.GetByPatientAsync(patient.PatientId, ct));
    }

    /// <summary>Get a specific patient's emergency requests (Operator / Admin only).</summary>
    [HttpGet("by-patient/{patientId:int}")]
    [Authorize(Roles = Roles.Operator + "," + Roles.Admin)]
    [ProducesResponseType(typeof(List<EmergencyRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EmergencyRequestDto>>> GetByPatient(int patientId, CancellationToken ct)
        => Ok(await _requests.GetByPatientAsync(patientId, ct));

    /// <summary>Cancel an emergency request (owner patient only, before an ambulance is assigned).</summary>
    [HttpPost("{requestId:long}/cancel")]
    [Authorize(Roles = Roles.Patient)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(long requestId, CancellationToken ct)
    {
        await _requests.CancelAsync(requestId, GetUserId(), ct);
        return NoContent();
    }

    /// <summary>Change the status of an emergency request (staff roles only, valid transitions only).</summary>
    [HttpPost("{requestId:long}/status")]
    [Authorize(Roles = Roles.Operator + "," + Roles.Paramedic + "," + Roles.Admin)]
    [ProducesResponseType(typeof(EmergencyRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmergencyRequestDto>> ChangeStatus(
        long requestId, [FromBody] ChangeStatusRequest body, CancellationToken ct)
    {
        // بنجيب الطلب الأول علشان لو مش موجود يرجّع 404 (الـ Service بيرجّع false في الحالتين)
        var current = await _requests.GetByIdAsync(requestId, ct);

        var userRole = User.FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedAccessException("User role was not found in the access token.");

        var changed = await _requests.ChangeStatusAsync(
            requestId,
            body.NewStatus,
            GetUserId(),
            userRole,
            ct);

        if (!changed)
            throw new InvalidOperationException(
                $"Cannot change status from {current.CurrentStatus} to {body.NewStatus}.");

        return Ok(await _requests.GetByIdAsync(requestId, ct));
    }

    // ================= Helpers =================

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User id was not found in the access token.");

    private async Task<PatientDto> GetCurrentPatientAsync(CancellationToken ct) =>
        await _patients.GetByUserIdAsync(GetUserId(), ct)
        ?? throw new UnauthorizedAccessException("No patient account is linked to the current user.");
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
