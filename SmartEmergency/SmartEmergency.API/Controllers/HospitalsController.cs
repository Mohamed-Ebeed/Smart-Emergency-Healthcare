
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.API.Extensions;
using SmartEmergency.Application.Hospitals.DTOs;
using SmartEmergency.Application.Hospitals.Interfaces;
using SmartEmergency.Domain.Common;
using SmartEmergency.Domain.Enums;
using System.Security.Claims;

namespace SmartEmergency.API.Controllers;

[ApiController]
[Route("api/hospitals")]
[Authorize]
[TypeFilter(typeof(ApiExceptionFilter))]
[Produces("application/json")]
public class HospitalsController : ControllerBase
{
    private const string StaffRoles =
        Roles.Operator + "," +
        Roles.HospitalStaff + "," +
        Roles.Paramedic + "," +
        Roles.Doctor + "," +
        Roles.Admin;

    private readonly IHospitalService _hospitals;
    private readonly IDepartmentService _departments;
    private readonly IHospitalBedService _beds;
    private readonly IStaffProfileService _staff;

    public HospitalsController(
        IHospitalService hospitals,
        IDepartmentService departments,
        IHospitalBedService beds,
        IStaffProfileService staff)
    {
        _hospitals = hospitals;
        _departments = departments;
        _beds = beds;
        _staff = staff;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(List<HospitalDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HospitalDto>>> GetAll(
        [FromQuery] bool activeOnly = true,
        CancellationToken ct = default)
    {
        return Ok(await _hospitals.GetAllAsync(activeOnly, ct));
    }

    [HttpGet("{hospitalId:int}")]
    [ProducesResponseType(
        typeof(HospitalDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HospitalDto>> GetById(
        int hospitalId,
        CancellationToken ct)
    {
        return Ok(await GetHospitalOrThrowAsync(hospitalId, ct));
    }

    [HttpGet("{hospitalId:int}/departments")]
    [Authorize(Roles = StaffRoles)]
    [ProducesResponseType(
        typeof(List<DepartmentDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<DepartmentDto>>> GetDepartments(
        int hospitalId,
        [FromQuery] bool activeOnly = true,
        CancellationToken ct = default)
    {
        await GetHospitalOrThrowAsync(hospitalId, ct);
        await EnsureCanAccessHospitalAsync(hospitalId, ct);

        return Ok(await _departments.GetByHospitalAsync(
            hospitalId,
            activeOnly,
            ct));
    }

    [HttpGet("{hospitalId:int}/beds")]
    [Authorize(Roles = StaffRoles)]
    [ProducesResponseType(
        typeof(List<HospitalBedDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HospitalBedDto>>> GetBeds(
        int hospitalId,
        [FromQuery] int? departmentId = null,
        [FromQuery] bool availableOnly = false,
        CancellationToken ct = default)
    {
        await GetHospitalOrThrowAsync(hospitalId, ct);
        await EnsureCanAccessHospitalAsync(hospitalId, ct);

        var beds = await _beds.GetByHospitalAsync(
            hospitalId,
            departmentId,
            ct);

        if (availableOnly)
        {
            beds = beds
                .Where(b => b.BedStatus == BedStatus.Available)
                .ToList();
        }

        return Ok(beds);
    }

    private async Task EnsureCanAccessHospitalAsync(
        int hospitalId,
        CancellationToken ct)
    {
        if (User.IsInRole(Roles.Admin) ||
            User.IsInRole(Roles.Operator))
        {
            return;
        }

        if (User.IsInRole(Roles.HospitalStaff))
        {
            var staffMembers =
                await _staff.GetHospitalStaffByHospitalAsync(
                    hospitalId,
                    ct);

            if (staffMembers.Any(s =>
                    s.UserId == GetUserId() && s.IsActive))
            {
                return;
            }
        }

        if (User.IsInRole(Roles.Paramedic))
        {
            var paramedics =
                await _staff.GetParamedicsByHospitalAsync(
                    hospitalId,
                    ct);

            if (paramedics.Any(p =>
                    p.UserId == GetUserId() && p.IsActive))
            {
                return;
            }
        }

        if (User.IsInRole(Roles.Doctor))
        {
            var doctors =
                await _staff.GetDoctorsByHospitalAsync(
                    hospitalId,
                    ct);

            if (doctors.Any(d =>
                    d.UserId == GetUserId() && d.IsActive))
            {
                return;
            }
        }

        throw new UnauthorizedAccessException(
            "You are not allowed to access data of this hospital.");
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedAccessException(
                   "User ID was not found.");
    }
    private async Task<HospitalDto> GetHospitalOrThrowAsync(
        int hospitalId,
        CancellationToken ct)
    {
        return await _hospitals.GetByIdAsync(hospitalId, ct)
               ?? throw new KeyNotFoundException(
                   $"Hospital {hospitalId} was not found.");
    }
}
