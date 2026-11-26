using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.Application.Patients.DTOs;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Domain.Common;

namespace SmartEmergency.API.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}")]
[Authorize]
[Tags("Medical Profile")]
[Produces("application/json")]
public class MedicalProfileController : PatientApiControllerBase
{
    private static readonly string[] MedicalEditRoles = { Roles.Doctor, Roles.Admin };

    private static readonly string[] ContactEditRoles = { Roles.Admin };

    private readonly IMedicalProfileService _profiles;

    public MedicalProfileController(IPatientService patients, IMedicalProfileService profiles) : base(patients)
        => _profiles = profiles;


    [HttpGet("medical-profile")]
    [EndpointSummary("Get medical profile")]
    [ProducesResponseType(typeof(MedicalProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicalProfileDto>> GetProfile(int patientId, CancellationToken ct)
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: false, ct);
        if (denied is not null) return denied;

        var profile = await _profiles.GetAsync(patientId, ct);
        if (profile is null)
            return NotFound(Problem404("This patient has no medical profile yet."));

        return Ok(profile);
    }

    [HttpPut("medical-profile")]
    [EndpointSummary("Create or update medical profile")]
    [EndpointDescription("Creates the profile if the patient doesn't have one, otherwise updates it (one profile per patient).")]
    [ProducesResponseType(typeof(MedicalProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> SaveProfile(int patientId, UpdateMedicalProfileDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        var exists = await _profiles.GetAsync(patientId, ct) is not null;
        var profile = exists
            ? await _profiles.UpdateAsync(patientId, dto, ct)
            : await _profiles.CreateAsync(patientId, dto, ct);

        return Ok(profile);
    });


    [HttpGet("chronic-diseases")]
    [EndpointSummary("Get chronic diseases")]
    [ProducesResponseType(typeof(IReadOnlyList<PatientChronicDiseaseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetChronicDiseases(int patientId, CancellationToken ct)
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: false, ct);
        if (denied is not null) return denied;

        return Ok(await _profiles.GetChronicDiseasesAsync(patientId, ct));
    }

    [HttpPost("chronic-diseases")]
    [EndpointSummary("Add chronic disease")]
    [ProducesResponseType(typeof(PatientChronicDiseaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public Task<ActionResult> AddChronicDisease(int patientId, AddPatientChronicDiseaseDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return Ok(await _profiles.AddChronicDiseaseAsync(patientId, dto, ct));
    });

    [HttpPut("chronic-diseases/{chronicDiseaseId:int}")]
    [EndpointSummary("Update chronic disease")]
    [ProducesResponseType(typeof(PatientChronicDiseaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> UpdateChronicDisease(int patientId, int chronicDiseaseId, UpdatePatientChronicDiseaseDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        var item = await _profiles.UpdateChronicDiseaseAsync(patientId, chronicDiseaseId, dto, ct);
        return item is null ? NotFound(Problem404("Chronic disease not found for this patient.")) : Ok(item);
    });

    [HttpDelete("chronic-diseases/{chronicDiseaseId:int}")]
    [EndpointSummary("Remove chronic disease")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> RemoveChronicDisease(int patientId, int chronicDiseaseId, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return await _profiles.RemoveChronicDiseaseAsync(patientId, chronicDiseaseId, ct)
            ? NoContent()
            : NotFound(Problem404("Chronic disease not found for this patient."));
    });

    [HttpGet("allergies")]
    [EndpointSummary("Get allergies")]
    [ProducesResponseType(typeof(IReadOnlyList<PatientAllergyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAllergies(int patientId, CancellationToken ct)
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: false, ct);
        if (denied is not null) return denied;

        return Ok(await _profiles.GetAllergiesAsync(patientId, ct));
    }

    [HttpPost("allergies")]
    [EndpointSummary("Add allergy")]
    [ProducesResponseType(typeof(PatientAllergyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public Task<ActionResult> AddAllergy(int patientId, AddPatientAllergyDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return Ok(await _profiles.AddAllergyAsync(patientId, dto, ct));
    });

    [HttpPut("allergies/{allergyId:int}")]
    [EndpointSummary("Update allergy")]
    [ProducesResponseType(typeof(PatientAllergyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> UpdateAllergy(int patientId, int allergyId, UpdatePatientAllergyDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        var item = await _profiles.UpdateAllergyAsync(patientId, allergyId, dto, ct);
        return item is null ? NotFound(Problem404("Allergy not found for this patient.")) : Ok(item);
    });

    [HttpDelete("allergies/{allergyId:int}")]
    [EndpointSummary("Remove allergy")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> RemoveAllergy(int patientId, int allergyId, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return await _profiles.RemoveAllergyAsync(patientId, allergyId, ct)
            ? NoContent()
            : NotFound(Problem404("Allergy not found for this patient."));
    });


    [HttpGet("medications")]
    [EndpointSummary("Get medications")]
    [ProducesResponseType(typeof(IReadOnlyList<PatientMedicationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetMedications(int patientId, CancellationToken ct)
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: false, ct);
        if (denied is not null) return denied;

        return Ok(await _profiles.GetMedicationsAsync(patientId, ct));
    }

    [HttpPost("medications")]
    [EndpointSummary("Add medication")]
    [ProducesResponseType(typeof(PatientMedicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public Task<ActionResult> AddMedication(int patientId, AddPatientMedicationDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return Ok(await _profiles.AddMedicationAsync(patientId, dto, ct));
    });

    [HttpPut("medications/{medicationId:int}")]
    [EndpointSummary("Update medication")]
    [ProducesResponseType(typeof(PatientMedicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> UpdateMedication(int patientId, int medicationId, UpdatePatientMedicationDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        var item = await _profiles.UpdateMedicationAsync(patientId, medicationId, dto, ct);
        return item is null ? NotFound(Problem404("Medication not found for this patient.")) : Ok(item);
    });

    [HttpDelete("medications/{medicationId:int}")]
    [EndpointSummary("Remove medication")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> RemoveMedication(int patientId, int medicationId, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, MedicalEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return await _profiles.RemoveMedicationAsync(patientId, medicationId, ct)
            ? NoContent()
            : NotFound(Problem404("Medication not found for this patient."));
    });

    [HttpGet("emergency-contacts")]
    [EndpointSummary("Get emergency contacts")]
    [ProducesResponseType(typeof(IReadOnlyList<EmergencyContactDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetContacts(int patientId, CancellationToken ct)
    {
        var denied = await CheckAccessAsync(patientId, ContactEditRoles, isEdit: false, ct);
        if (denied is not null) return denied;

        return Ok(await _profiles.GetEmergencyContactsAsync(patientId, ct));
    }

    [HttpPost("emergency-contacts")]
    [EndpointSummary("Add emergency contact")]
    [ProducesResponseType(typeof(EmergencyContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<ActionResult> AddContact(int patientId, SaveEmergencyContactDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, ContactEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return Ok(await _profiles.AddEmergencyContactAsync(patientId, dto, ct));
    });

    [HttpPut("emergency-contacts/{contactId:int}")]
    [EndpointSummary("Update emergency contact")]
    [ProducesResponseType(typeof(EmergencyContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> UpdateContact(int patientId, int contactId, SaveEmergencyContactDto dto, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, ContactEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        var item = await _profiles.UpdateEmergencyContactAsync(patientId, contactId, dto, ct);
        return item is null ? NotFound(Problem404("Contact not found for this patient.")) : Ok(item);
    });

    [HttpPut("emergency-contacts/{contactId:int}/primary")]
    [EndpointSummary("Make contact the primary one")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> SetPrimaryContact(int patientId, int contactId, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, ContactEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return await _profiles.SetPrimaryEmergencyContactAsync(patientId, contactId, ct)
            ? NoContent()
            : NotFound(Problem404("Contact not found for this patient."));
    });

    [HttpDelete("emergency-contacts/{contactId:int}")]
    [EndpointSummary("Delete emergency contact")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public Task<ActionResult> DeleteContact(int patientId, int contactId, CancellationToken ct) => HandleAsync(async () =>
    {
        var denied = await CheckAccessAsync(patientId, ContactEditRoles, isEdit: true, ct);
        if (denied is not null) return denied;

        return await _profiles.RemoveEmergencyContactAsync(patientId, contactId, ct)
            ? NoContent()
            : NotFound(Problem404("Contact not found for this patient."));
    });
}