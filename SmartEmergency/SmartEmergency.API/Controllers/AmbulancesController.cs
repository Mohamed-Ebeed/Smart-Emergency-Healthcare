
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.API.Extensions;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.API.Controllers;

[ApiController]
[Route("api/ambulances")]
[Authorize(Roles = Roles.Operator + "," + Roles.HospitalStaff + "," + Roles.Paramedic + "," + Roles.Doctor + "," + Roles.Admin)]
[TypeFilter(typeof(ApiExceptionFilter))]
[Produces("application/json")]
public class AmbulancesController : ControllerBase
{
    private readonly IAmbulanceService _ambulances;
    private readonly IHospitalService _hospitals;
    private readonly IStaffProfileService _staff;

    public AmbulancesController(
        IAmbulanceService ambulances,
        IHospitalService hospitals,
        IStaffProfileService staff)
    {
        _ambulances = ambulances;
        _hospitals = hospitals;
        _staff = staff;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<AmbulanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<AmbulanceDto>>> GetByHospital(
        [FromQuery, Required] int hospitalId,
        [FromQuery] bool availableOnly = false,
        CancellationToken ct = default)
    {
        if (await _hospitals.GetByIdAsync(hospitalId, ct) is null)
            throw new KeyNotFoundException($"Hospital {hospitalId} was not found.");

        await EnsureCanAccessHospitalAsync(hospitalId, ct);

        var ambulances = await _ambulances.GetByHospitalAsync(hospitalId, ct);

        if (availableOnly)
        {
            ambulances = ambulances
                .Where(a => a.AmbulanceStatus == AmbulanceStatus.Available)
                .ToList();
        }

        return Ok(ambulances);
    }

    [HttpGet("{ambulanceId:int}")]
    [ProducesResponseType(typeof(AmbulanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AmbulanceDto>> GetById(
        int ambulanceId,
        CancellationToken ct)
    {
        var ambulance = await GetAmbulanceOrThrowAsync(ambulanceId, ct);

        await EnsureCanAccessHospitalAsync(ambulance.HospitalId, ct);

        return Ok(ambulance);
    }

    [HttpPut("{ambulanceId:int}/status")]
    [Authorize(Roles = Roles.Operator + "," + Roles.HospitalStaff + "," + Roles.Admin)]
    [ProducesResponseType(typeof(AmbulanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AmbulanceDto>> ChangeStatus(
        int ambulanceId,
        [FromBody] ChangeAmbulanceStatusRequest body,
        CancellationToken ct)
    {
        var ambulance = await GetAmbulanceOrThrowAsync(ambulanceId, ct);

        await EnsureCanManageAmbulanceAsync(ambulance.HospitalId, ct);

        var changed = await _ambulances.ChangeStatusAsync(
            ambulanceId,
            body.Status,
            ct);

        if (!changed)
            throw new KeyNotFoundException(
                $"Ambulance {ambulanceId} was not found.");

        return Ok(await GetAmbulanceOrThrowAsync(ambulanceId, ct));
    }

    private async Task EnsureCanAccessHospitalAsync(
        int hospitalId,
        CancellationToken ct)
    {
        var user = User;
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User ID not found.");

        if (user.IsInRole(Roles.Admin) || user.IsInRole(Roles.Operator))
            return;

        if (user.IsInRole(Roles.HospitalStaff))
        {
            var staff = await _staff.GetHospitalStaffByHospitalAsync(
                hospitalId, ct);

            if (staff.Any(s => s.UserId == userId && s.IsActive))
                return;
        }

        if (user.IsInRole(Roles.Paramedic))
        {
            var paramedics = await _staff.GetParamedicsByHospitalAsync(
                hospitalId, ct);

            if (paramedics.Any(p => p.UserId == userId && p.IsActive))
                return;
        }

        if (user.IsInRole(Roles.Doctor))
        {
            var doctors = await _staff.GetDoctorsByHospitalAsync(
                hospitalId, ct);

            if (doctors.Any(d => d.UserId == userId && d.IsActive))
                return;
        }

        throw new UnauthorizedAccessException(
            "You are not allowed to access ambulances of this hospital.");
    }

    private async Task EnsureCanManageAmbulanceAsync(
        int hospitalId,
        CancellationToken ct)
    {
        if (User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Operator))
            return;

        if (User.IsInRole(Roles.HospitalStaff))
        {
            await EnsureCanAccessHospitalAsync(hospitalId, ct);
            return;
        }

        throw new UnauthorizedAccessException(
            "You are not allowed to change this ambulance status.");
    }

    private async Task<AmbulanceDto> GetAmbulanceOrThrowAsync(
        int ambulanceId,
        CancellationToken ct)
    {
        return await _ambulances.GetByIdAsync(ambulanceId, ct)
            ?? throw new KeyNotFoundException(
                $"Ambulance {ambulanceId} was not found.");
    }
}

public class ChangeAmbulanceStatusRequest
{
    [Required]
    [EnumDataType(typeof(AmbulanceStatus))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AmbulanceStatus Status { get; set; }
}
