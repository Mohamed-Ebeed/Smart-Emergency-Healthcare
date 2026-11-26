
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.Application.Patients.DTOs;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Domain.Common;

namespace SmartEmergency.API.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize]
[Tags("Patients")]
[Produces("application/json")]
public class PatientsController : PatientApiControllerBase
{
    private static readonly string[] EditRoles = { Roles.Admin };

    public PatientsController(IPatientService patients) : base(patients) { }

    [HttpGet("me")]
    [Authorize(Roles = Roles.Patient)]
    [EndpointSummary("Get my patient profile")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetMe(CancellationToken ct)
    {
        var patient = await Patients.GetByUserIdAsync(CurrentUserId!, ct);
        if (patient is null)
            return NotFound(Problem404("No patient record for this account."));

        return Ok(patient);
    }

    [HttpPut("me")]
    [Authorize(Roles = Roles.Patient)]
    [EndpointSummary("Update my patient information")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> UpdateMe(UpdatePatientDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var patient = await Patients.GetByUserIdAsync(CurrentUserId!, ct);
        if (patient is null)
            return NotFound(Problem404("No patient record for this account."));

        return Ok(await Patients.UpdateAsync(patient.PatientId, dto, ct));
    });

    [HttpGet("{patientId:int}")]
    [EndpointSummary("Get patient details by id")]
    [EndpointDescription("Patient can open his own record only. Staff roles can open any patient.")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetById(int patientId, CancellationToken ct)
    {
        var denied = await CheckAccessAsync(patientId, EditRoles, allowPatient: true, ct);
        if (denied is not null) return denied;

        return Ok(await Patients.GetByIdAsync(patientId, ct));
    }

    [HttpPut("{patientId:int}")]
    [EndpointSummary("Update patient information")]
    [EndpointDescription("Allowed for the patient himself or Admin.")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> Update(int patientId, UpdatePatientDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, EditRoles, allowPatient: true, ct);
        if (denied is not null) return denied;

        return Ok(await Patients.UpdateAsync(patientId, dto, ct));
    });
}
