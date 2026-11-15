using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEmergency.Application.Patients.DTOs;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Web.Models;

namespace SmartEmergency.Web.Controllers;

// الـ Controller مفيهوش Business Logic: بيجيب الـ patientId من المستخدم المسجّل،
// بيستدعي الـ Services، وبيحوّل النتايج (null / false / استثناءات) لرسائل للمستخدم.
[Authorize]
public class PatientController : Controller
{
    private readonly IPatientService _patients;
    private readonly IMedicalProfileService _profiles;

    public PatientController(IPatientService patients, IMedicalProfileService profiles)
    {
        _patients = patients;
        _profiles = profiles;
    }

    // ================= Pages =================

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var patient = await GetCurrentPatientAsync(ct);
        if (patient is null) return NoPatientAccount();

        var model = new PatientDashboardViewModel
        {
            Patient = patient,
            Profile = await _profiles.GetAsync(patient.PatientId, ct),
            ChronicDiseases = await _profiles.GetChronicDiseasesAsync(patient.PatientId, ct),
            Allergies = await _profiles.GetAllergiesAsync(patient.PatientId, ct),
            Medications = await _profiles.GetMedicationsAsync(patient.PatientId, ct),
            EmergencyContacts = await _profiles.GetEmergencyContactsAsync(patient.PatientId, ct)
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditPatient(CancellationToken ct)
    {
        var patient = await GetCurrentPatientAsync(ct);
        if (patient is null) return NoPatientAccount();

        return View(new UpdatePatientDto
        {
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Address = patient.Address
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPatient(UpdatePatientDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);

        var patient = await GetCurrentPatientAsync(ct);
        if (patient is null) return NoPatientAccount();

        try
        {
            var updated = await _patients.UpdateAsync(patient.PatientId, dto, ct);
            if (updated is null)
            {
                TempData["Error"] = "Patient was not found.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Success"] = "Your information was updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    // ================= Medical Profile =================

    // الـ Profile واحد بس لكل مريض: لو موجود بنعدّله، ولو مش موجود بننشئه
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SaveProfile(CreateMedicalProfileDto dto, CancellationToken ct) =>
        RunAsync(async patientId =>
        {
            if (await _profiles.GetAsync(patientId, ct) is null)
                await _profiles.CreateAsync(patientId, dto, ct);
            else
                await _profiles.UpdateAsync(patientId, new UpdateMedicalProfileDto
                {
                    BloodType = dto.BloodType,
                    PrimaryConditions = dto.PrimaryConditions,
                    CurrentNotes = dto.CurrentNotes
                }, ct);
            return true;
        }, "Medical profile saved.", "Medical profile was not found.");

    // ================= Chronic Diseases =================

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> AddChronicDisease(AddPatientChronicDiseaseDto dto, CancellationToken ct) =>
        RunAsync(async id => { await _profiles.AddChronicDiseaseAsync(id, dto, ct); return true; },
                 "Chronic disease added.", "Patient was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> UpdateChronicDisease(int chronicDiseaseId, UpdatePatientChronicDiseaseDto dto, CancellationToken ct) =>
        RunAsync(async id => await _profiles.UpdateChronicDiseaseAsync(id, chronicDiseaseId, dto, ct) is not null,
                 "Chronic disease updated.", "Chronic disease was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> RemoveChronicDisease(int chronicDiseaseId, CancellationToken ct) =>
        RunAsync(id => _profiles.RemoveChronicDiseaseAsync(id, chronicDiseaseId, ct),
                 "Chronic disease removed.", "Chronic disease was not found.");

    // ================= Allergies =================

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> AddAllergy(AddPatientAllergyDto dto, CancellationToken ct) =>
        RunAsync(async id => { await _profiles.AddAllergyAsync(id, dto, ct); return true; },
                 "Allergy added.", "Patient was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> UpdateAllergy(int allergyId, UpdatePatientAllergyDto dto, CancellationToken ct) =>
        RunAsync(async id => await _profiles.UpdateAllergyAsync(id, allergyId, dto, ct) is not null,
                 "Allergy updated.", "Allergy was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> RemoveAllergy(int allergyId, CancellationToken ct) =>
        RunAsync(id => _profiles.RemoveAllergyAsync(id, allergyId, ct),
                 "Allergy removed.", "Allergy was not found.");

    // ================= Medications =================

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> AddMedication(AddPatientMedicationDto dto, CancellationToken ct) =>
        RunAsync(async id => { await _profiles.AddMedicationAsync(id, dto, ct); return true; },
                 "Medication added.", "Patient was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> UpdateMedication(int medicationId, UpdatePatientMedicationDto dto, CancellationToken ct) =>
        RunAsync(async id => await _profiles.UpdateMedicationAsync(id, medicationId, dto, ct) is not null,
                 "Medication updated.", "Medication was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> RemoveMedication(int medicationId, CancellationToken ct) =>
        RunAsync(id => _profiles.RemoveMedicationAsync(id, medicationId, ct),
                 "Medication removed.", "Medication was not found.");

    // ================= Emergency Contacts =================

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> AddContact(SaveEmergencyContactDto dto, CancellationToken ct) =>
        RunAsync(async id => { await _profiles.AddEmergencyContactAsync(id, dto, ct); return true; },
                 "Emergency contact added.", "Patient was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> UpdateContact(int emergencyContactId, SaveEmergencyContactDto dto, CancellationToken ct) =>
        RunAsync(async id => await _profiles.UpdateEmergencyContactAsync(id, emergencyContactId, dto, ct) is not null,
                 "Emergency contact updated.", "Emergency contact was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> RemoveContact(int emergencyContactId, CancellationToken ct) =>
        RunAsync(id => _profiles.RemoveEmergencyContactAsync(id, emergencyContactId, ct),
                 "Emergency contact removed.", "Emergency contact was not found.");

    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SetPrimaryContact(int emergencyContactId, CancellationToken ct) =>
        RunAsync(id => _profiles.SetPrimaryEmergencyContactAsync(id, emergencyContactId, ct),
                 "Primary contact updated.", "Emergency contact was not found.");

    // ================= Helpers =================

    // النقطة 1: الـ patientId بييجي من المستخدم المسجّل دخوله (GetByUserIdAsync)، مش من الـ URL
    private async Task<PatientDto?> GetCurrentPatientAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return null;
        return await _patients.GetByUserIdAsync(userId, ct);
    }

    private IActionResult NoPatientAccount()
    {
        TempData["Error"] = "No patient account is linked to your user.";
        return RedirectToAction("Index", "Home");
    }

    // النقطة 2: بنمسك null / false / الاستثناءات ونحوّلها لرسالة مفهومة
    private async Task<IActionResult> RunAsync(Func<int, Task<bool>> operation, string successMessage, string notFoundMessage)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = string.Join(" | ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index));
        }

        var patient = await GetCurrentPatientAsync(HttpContext.RequestAborted);
        if (patient is null) return NoPatientAccount();

        try
        {
            var done = await operation(patient.PatientId);
            if (done) TempData["Success"] = successMessage;
            else TempData["Error"] = notFoundMessage;          // null / false
        }
        catch (ValidationException ex)         { TempData["Error"] = ex.Message; }
        catch (InvalidOperationException ex)   { TempData["Error"] = ex.Message; }   // مكرر، مثلاً Profile موجود بالفعل
        catch (KeyNotFoundException)           { TempData["Error"] = "The requested data was not found."; }

        return RedirectToAction(nameof(Index));
    }
}
