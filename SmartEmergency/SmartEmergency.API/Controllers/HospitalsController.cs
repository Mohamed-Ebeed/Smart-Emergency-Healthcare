using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.API.Extensions;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.API.Controllers;

/// <summary>
/// Hospitals read endpoints. مفيش Business Logic هنا: بننده الـ Services الموجودة،
/// والـ ApiExceptionFilter بيحوّل الاستثناءات لـ HTTP status.
/// </summary>
[ApiController]
[Route("api/hospitals")]
[Authorize]
[TypeFilter(typeof(ApiExceptionFilter))]
[Produces("application/json")]
public class HospitalsController : ControllerBase
{
    // الأدوار اللي مسموح لها تشوف بيانات تشغيلية (الأقسام والأسرّة)
    private const string StaffRoles =
        Roles.Operator + "," + Roles.HospitalStaff + "," + Roles.Paramedic + "," + Roles.Doctor + "," + Roles.Admin;

    private readonly IHospitalService _hospitals;
    private readonly IDepartmentService _departments;
    private readonly IHospitalBedService _beds;

    public HospitalsController(IHospitalService hospitals, IDepartmentService departments, IHospitalBedService beds)
    {
        _hospitals = hospitals;
        _departments = departments;
        _beds = beds;
    }

    /// <summary>Get hospitals (active only by default).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<HospitalDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HospitalDto>>> GetAll([FromQuery] bool activeOnly = true, CancellationToken ct = default)
        => Ok(await _hospitals.GetAllAsync(activeOnly, ct));

    /// <summary>Get hospital details.</summary>
    [HttpGet("{hospitalId:int}")]
    [ProducesResponseType(typeof(HospitalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HospitalDto>> GetById(int hospitalId, CancellationToken ct)
        => Ok(await GetHospitalOrThrowAsync(hospitalId, ct));

    /// <summary>Get the departments of a hospital.</summary>
    [HttpGet("{hospitalId:int}/departments")]
    [Authorize(Roles = StaffRoles)]
    [ProducesResponseType(typeof(List<DepartmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<DepartmentDto>>> GetDepartments(
        int hospitalId, [FromQuery] bool activeOnly = true, CancellationToken ct = default)
    {
        await GetHospitalOrThrowAsync(hospitalId, ct);
        return Ok(await _departments.GetByHospitalAsync(hospitalId, activeOnly, ct));
    }

    /// <summary>
    /// Get the beds of a hospital (optionally one department). Use availableOnly=true to see free beds only.
    /// </summary>
    [HttpGet("{hospitalId:int}/beds")]
    [Authorize(Roles = StaffRoles)]
    [ProducesResponseType(typeof(List<HospitalBedDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HospitalBedDto>>> GetBeds(
        int hospitalId, [FromQuery] int? departmentId = null, [FromQuery] bool availableOnly = false,
        CancellationToken ct = default)
    {
        await GetHospitalOrThrowAsync(hospitalId, ct);

        var beds = await _beds.GetByHospitalAsync(hospitalId, departmentId, ct);
        if (availableOnly)
            beds = beds.Where(b => b.BedStatus == BedStatus.Available).ToList();

        return Ok(beds);
    }

    // الـ Service بيرجّع null لو المستشفى مش موجودة، فبنحوّلها لـ 404
    private async Task<HospitalDto> GetHospitalOrThrowAsync(int hospitalId, CancellationToken ct) =>
        await _hospitals.GetByIdAsync(hospitalId, ct)
        ?? throw new KeyNotFoundException($"Hospital {hospitalId} was not found.");
}
