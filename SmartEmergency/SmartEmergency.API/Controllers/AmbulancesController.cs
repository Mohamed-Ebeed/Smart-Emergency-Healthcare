using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.API.Extensions;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.API.Controllers;

/// <summary>
/// Ambulances endpoints. الأدوار التشغيلية بس هي اللي تشوف وتعدّل (المريض مالوش دخل بالإسعاف).
/// </summary>
[ApiController]
[Route("api/ambulances")]
[Authorize(Roles = Roles.Operator + "," + Roles.HospitalStaff + "," + Roles.Paramedic + "," + Roles.Doctor + "," + Roles.Admin)]
[TypeFilter(typeof(ApiExceptionFilter))]
[Produces("application/json")]
public class AmbulancesController : ControllerBase
{
    private readonly IAmbulanceService _ambulances;
    private readonly IHospitalService _hospitals;

    public AmbulancesController(IAmbulanceService ambulances, IHospitalService hospitals)
    {
        _ambulances = ambulances;
        _hospitals = hospitals;
    }

    /// <summary>
    /// Get the ambulances of a hospital. Use availableOnly=true to get the available ones only.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AmbulanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<AmbulanceDto>>> GetByHospital(
        [FromQuery, Required] int hospitalId, [FromQuery] bool availableOnly = false, CancellationToken ct = default)
    {
        if (await _hospitals.GetByIdAsync(hospitalId, ct) is null)
            throw new KeyNotFoundException($"Hospital {hospitalId} was not found.");

        var ambulances = await _ambulances.GetByHospitalAsync(hospitalId, ct);
        if (availableOnly)
            ambulances = ambulances.Where(a => a.AmbulanceStatus == AmbulanceStatus.Available).ToList();

        return Ok(ambulances);
    }

    /// <summary>Get ambulance details.</summary>
    [HttpGet("{ambulanceId:int}")]
    [ProducesResponseType(typeof(AmbulanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AmbulanceDto>> GetById(int ambulanceId, CancellationToken ct)
        => Ok(await GetAmbulanceOrThrowAsync(ambulanceId, ct));

    /// <summary>Update ambulance availability (Available / Busy / Offline).</summary>
    [HttpPut("{ambulanceId:int}/status")]
    [Authorize(Roles = Roles.Operator + "," + Roles.HospitalStaff + "," + Roles.Paramedic + "," + Roles.Admin)]
    [ProducesResponseType(typeof(AmbulanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AmbulanceDto>> ChangeStatus(
        int ambulanceId, [FromBody] ChangeAmbulanceStatusRequest body, CancellationToken ct)
    {
        // الـ Service بيرجّع false لو الإسعاف مش موجودة
        var changed = await _ambulances.ChangeStatusAsync(ambulanceId, body.Status, ct);
        if (!changed)
            throw new KeyNotFoundException($"Ambulance {ambulanceId} was not found.");

        return Ok(await GetAmbulanceOrThrowAsync(ambulanceId, ct));
    }

    private async Task<AmbulanceDto> GetAmbulanceOrThrowAsync(int ambulanceId, CancellationToken ct) =>
        await _ambulances.GetByIdAsync(ambulanceId, ct)
        ?? throw new KeyNotFoundException($"Ambulance {ambulanceId} was not found.");
}

public class ChangeAmbulanceStatusRequest
{
    [Required, EnumDataType(typeof(AmbulanceStatus))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AmbulanceStatus Status { get; set; }
}
