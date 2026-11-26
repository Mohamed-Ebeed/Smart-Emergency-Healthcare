using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.Application.Patients.DTOs;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Domain.Common;

namespace SmartEmergency.API.Controllers;

public abstract class PatientApiControllerBase : ControllerBase
{
    protected static readonly string[] StaffRoles =
        { Roles.Doctor, Roles.Paramedic, Roles.Operator, Roles.HospitalStaff, Roles.Admin };

    protected readonly IPatientService Patients;

    protected PatientApiControllerBase(IPatientService patients) => Patients = patients;

    protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    protected async Task<ActionResult?> CheckAccessAsync(int patientId, string[] editRoles, bool isEdit, CancellationToken ct)
    {
        var patient = await Patients.GetByIdAsync(patientId, ct);
        if (patient is null)
            return NotFound(Problem404($"Patient {patientId} was not found."));

        if (User.IsInRole(Roles.Patient))
            return patient.UserId == CurrentUserId ? null : Forbid();

        var allowedRoles = isEdit ? editRoles : StaffRoles;
        return allowedRoles.Any(User.IsInRole) ? null : Forbid();
    }

    protected async Task<ActionResult> HandleAsync(Func<Task<ActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ProblemDetails { Status = 400, Title = "Validation failed.", Detail = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Problem404(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Status = 409, Title = "Request conflict.", Detail = ex.Message });
        }
    }

    protected static ProblemDetails Problem404(string detail) =>
        new() { Status = 404, Title = "Not found.", Detail = detail };
}