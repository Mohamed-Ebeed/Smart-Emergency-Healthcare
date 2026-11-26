
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Domain.Common;

namespace SmartEmergency.API.Controllers;

public abstract class PatientApiControllerBase : ControllerBase
{
    protected static readonly string[] MedicalReadRoles =
        { Roles.Doctor, Roles.Paramedic, Roles.Admin };

    protected readonly IPatientService Patients;

    protected PatientApiControllerBase(IPatientService patients) => Patients = patients;

    protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);


    protected async Task<ActionResult?> CheckAccessAsync(
        int patientId,
        string[] allowedRoles,
        bool allowPatient,
        CancellationToken ct)
    {
        var patient = await Patients.GetByIdAsync(patientId, ct);

        if (patient is null)
            return NotFound(Problem404($"Patient {patientId} was not found."));

        // Patients can access their own records only when explicitly allowed.
        if (User.IsInRole(Roles.Patient))
        {
            var isOwner = patient.UserId == CurrentUserId;
            return allowPatient && isOwner ? null : Forbid();
        }

        // Other roles must be explicitly included in the allowed roles.
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
            return BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Validation failed.",
                Detail = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Problem404(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Status = 409,
                Title = "Request conflict.",
                Detail = ex.Message
            });
        }
    }

    protected static ProblemDetails Problem404(string detail) =>
        new() { Status = 404, Title = "Not found.", Detail = detail };
}
