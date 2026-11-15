using Microsoft.EntityFrameworkCore;
using SmartEmergency.Application.Common.Interfaces;
using SmartEmergency.Application.Patients.DTOs;
using SmartEmergency.Application.Patients.Interfaces;
using SmartEmergency.Domain.Entities;

namespace SmartEmergency.Application.Patients.Services;

public class MedicalProfileService : IMedicalProfileService
{
    private readonly IAppDbContext _db;

    public MedicalProfileService(IAppDbContext db) => _db = db;

    // =====================================================
    // Medical Profile
    // =====================================================

    public async Task<MedicalProfileDto?> GetAsync(int patientId, CancellationToken ct = default)
    {
        var profile = await _db.MedicalProfiles.AsNoTracking()
            .FirstOrDefaultAsync(m => m.PatientId == patientId, ct);
        return profile is null ? null : MapProfile(profile);
    }

    public async Task<MedicalProfileDto> CreateAsync(int patientId, CreateMedicalProfileDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);
        await EnsurePatientExistsAsync(patientId, ct);

        // one-to-one: المريض ليه Profile واحد بس
        if (await _db.MedicalProfiles.AnyAsync(m => m.PatientId == patientId, ct))
            throw new InvalidOperationException("This patient already has a medical profile.");

        var profile = new MedicalProfile
        {
            PatientId = patientId,
            BloodType = dto.BloodType,
            PrimaryConditions = dto.PrimaryConditions?.Trim(),
            CurrentNotes = dto.CurrentNotes?.Trim(),
            LastUpdatedAt = DateTime.UtcNow
        };

        _db.MedicalProfiles.Add(profile);
        await _db.SaveChangesAsync(ct);
        return MapProfile(profile);
    }

    public async Task<MedicalProfileDto> UpdateAsync(int patientId, UpdateMedicalProfileDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var profile = await _db.MedicalProfiles.FirstOrDefaultAsync(m => m.PatientId == patientId, ct)
            ?? throw new KeyNotFoundException($"Medical profile for patient {patientId} was not found.");

        profile.BloodType = dto.BloodType;
        profile.PrimaryConditions = dto.PrimaryConditions?.Trim();
        profile.CurrentNotes = dto.CurrentNotes?.Trim();
        profile.LastUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return MapProfile(profile);
    }

    // =====================================================
    // Chronic Diseases  (Catalog: ChronicDisease  |  Link: PatientChronicDisease)
    // =====================================================

    public async Task<IReadOnlyList<PatientChronicDiseaseDto>> GetChronicDiseasesAsync(int patientId, CancellationToken ct = default) =>
        await _db.PatientChronicDiseases.AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .Select(x => new PatientChronicDiseaseDto
            {
                ChronicDiseaseId = x.ChronicDiseaseId,
                Name = x.ChronicDisease.Name,
                Description = x.ChronicDisease.Description,
                DiagnosedAt = x.DiagnosedAt,
                Notes = x.Notes
            })
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<PatientChronicDiseaseDto> AddChronicDiseaseAsync(int patientId, AddPatientChronicDiseaseDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);
        await EnsurePatientExistsAsync(patientId, ct);

        var name = dto.Name.Trim();
        var key = name.ToLower();

        // الأسماء Unique في الـ Catalog، فنستخدم المرض لو موجود وإلا نضيفه
        var disease = await _db.ChronicDiseases.FirstOrDefaultAsync(c => c.Name.ToLower() == key, ct);
        if (disease is null)
        {
            disease = new ChronicDisease { Name = name, Description = dto.Description?.Trim() };
            _db.ChronicDiseases.Add(disease);
        }
        else if (await _db.PatientChronicDiseases.AnyAsync(x => x.PatientId == patientId && x.ChronicDiseaseId == disease.ChronicDiseaseId, ct))
        {
            throw new InvalidOperationException("This chronic disease is already added for the patient.");
        }

        var link = new PatientChronicDisease
        {
            PatientId = patientId,
            ChronicDisease = disease,
            DiagnosedAt = dto.DiagnosedAt,
            Notes = dto.Notes?.Trim()
        };
        _db.PatientChronicDiseases.Add(link);
        await _db.SaveChangesAsync(ct);

        return MapDisease(link, disease);
    }

    public async Task<PatientChronicDiseaseDto?> UpdateChronicDiseaseAsync(int patientId, int chronicDiseaseId, UpdatePatientChronicDiseaseDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var link = await _db.PatientChronicDiseases.Include(x => x.ChronicDisease)
            .FirstOrDefaultAsync(x => x.PatientId == patientId && x.ChronicDiseaseId == chronicDiseaseId, ct);
        if (link is null) return null;

        link.DiagnosedAt = dto.DiagnosedAt;
        link.Notes = dto.Notes?.Trim();
        await _db.SaveChangesAsync(ct);

        return MapDisease(link, link.ChronicDisease);
    }

    public async Task<bool> RemoveChronicDiseaseAsync(int patientId, int chronicDiseaseId, CancellationToken ct = default)
    {
        var link = await _db.PatientChronicDiseases
            .FirstOrDefaultAsync(x => x.PatientId == patientId && x.ChronicDiseaseId == chronicDiseaseId, ct);
        if (link is null) return false;

        _db.PatientChronicDiseases.Remove(link);   // بنمسح الربط بس، الـ Catalog يفضل زي ما هو
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // =====================================================
    // Allergies  (Catalog: Allergy  |  Link: PatientAllergy)
    // =====================================================

    public async Task<IReadOnlyList<PatientAllergyDto>> GetAllergiesAsync(int patientId, CancellationToken ct = default) =>
        await _db.PatientAllergies.AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .Select(x => new PatientAllergyDto
            {
                AllergyId = x.AllergyId,
                Name = x.Allergy.Name,
                Description = x.Allergy.Description,
                Severity = x.Severity,
                Reaction = x.Reaction
            })
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<PatientAllergyDto> AddAllergyAsync(int patientId, AddPatientAllergyDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);
        await EnsurePatientExistsAsync(patientId, ct);

        var name = dto.Name.Trim();
        var key = name.ToLower();

        var allergy = await _db.Allergies.FirstOrDefaultAsync(a => a.Name.ToLower() == key, ct);
        if (allergy is null)
        {
            allergy = new Allergy { Name = name, Description = dto.Description?.Trim() };
            _db.Allergies.Add(allergy);
        }
        else if (await _db.PatientAllergies.AnyAsync(x => x.PatientId == patientId && x.AllergyId == allergy.AllergyId, ct))
        {
            throw new InvalidOperationException("This allergy is already added for the patient.");
        }

        var link = new PatientAllergy
        {
            PatientId = patientId,
            Allergy = allergy,
            Severity = dto.Severity?.Trim(),
            Reaction = dto.Reaction?.Trim()
        };
        _db.PatientAllergies.Add(link);
        await _db.SaveChangesAsync(ct);

        return MapAllergy(link, allergy);
    }

    public async Task<PatientAllergyDto?> UpdateAllergyAsync(int patientId, int allergyId, UpdatePatientAllergyDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var link = await _db.PatientAllergies.Include(x => x.Allergy)
            .FirstOrDefaultAsync(x => x.PatientId == patientId && x.AllergyId == allergyId, ct);
        if (link is null) return null;

        link.Severity = dto.Severity?.Trim();
        link.Reaction = dto.Reaction?.Trim();
        await _db.SaveChangesAsync(ct);

        return MapAllergy(link, link.Allergy);
    }

    public async Task<bool> RemoveAllergyAsync(int patientId, int allergyId, CancellationToken ct = default)
    {
        var link = await _db.PatientAllergies
            .FirstOrDefaultAsync(x => x.PatientId == patientId && x.AllergyId == allergyId, ct);
        if (link is null) return false;

        _db.PatientAllergies.Remove(link);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // =====================================================
    // Medications  (Catalog: Medication  |  Link: PatientMedication)
    // =====================================================

    public async Task<IReadOnlyList<PatientMedicationDto>> GetMedicationsAsync(int patientId, CancellationToken ct = default) =>
        await _db.PatientMedications.AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .Select(x => new PatientMedicationDto
            {
                MedicationId = x.MedicationId,
                Name = x.Medication.Name,
                Description = x.Medication.Description,
                Dosage = x.Dosage,
                Frequency = x.Frequency,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            })
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task<PatientMedicationDto> AddMedicationAsync(int patientId, AddPatientMedicationDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);
        await EnsurePatientExistsAsync(patientId, ct);

        var name = dto.Name.Trim();
        var key = name.ToLower();

        var medication = await _db.Medications.FirstOrDefaultAsync(m => m.Name.ToLower() == key, ct);
        if (medication is null)
        {
            medication = new Medication { Name = name, Description = dto.Description?.Trim() };
            _db.Medications.Add(medication);
        }
        else if (await _db.PatientMedications.AnyAsync(x => x.PatientId == patientId && x.MedicationId == medication.MedicationId, ct))
        {
            throw new InvalidOperationException("This medication is already added for the patient.");
        }

        var link = new PatientMedication
        {
            PatientId = patientId,
            Medication = medication,
            Dosage = dto.Dosage?.Trim(),
            Frequency = dto.Frequency?.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
        _db.PatientMedications.Add(link);
        await _db.SaveChangesAsync(ct);

        return MapMedication(link, medication);
    }

    public async Task<PatientMedicationDto?> UpdateMedicationAsync(int patientId, int medicationId, UpdatePatientMedicationDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var link = await _db.PatientMedications.Include(x => x.Medication)
            .FirstOrDefaultAsync(x => x.PatientId == patientId && x.MedicationId == medicationId, ct);
        if (link is null) return null;

        link.Dosage = dto.Dosage?.Trim();
        link.Frequency = dto.Frequency?.Trim();
        link.StartDate = dto.StartDate;
        link.EndDate = dto.EndDate;
        await _db.SaveChangesAsync(ct);

        return MapMedication(link, link.Medication);
    }

    public async Task<bool> RemoveMedicationAsync(int patientId, int medicationId, CancellationToken ct = default)
    {
        var link = await _db.PatientMedications
            .FirstOrDefaultAsync(x => x.PatientId == patientId && x.MedicationId == medicationId, ct);
        if (link is null) return false;

        _db.PatientMedications.Remove(link);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // =====================================================
    // Emergency Contacts  (تابعة للـ Patient مباشرة)
    // =====================================================

    public async Task<IReadOnlyList<EmergencyContactDto>> GetEmergencyContactsAsync(int patientId, CancellationToken ct = default) =>
        await _db.EmergencyContacts.AsNoTracking()
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.IsPrimary).ThenBy(c => c.FullName)
            .Select(c => new EmergencyContactDto
            {
                EmergencyContactId = c.EmergencyContactId,
                PatientId = c.PatientId,
                FullName = c.FullName,
                Relationship = c.Relationship,
                PhoneNumber = c.PhoneNumber,
                IsPrimary = c.IsPrimary
            })
            .ToListAsync(ct);

    public async Task<EmergencyContactDto> AddEmergencyContactAsync(int patientId, SaveEmergencyContactDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);
        await EnsurePatientExistsAsync(patientId, ct);

        var existing = await _db.EmergencyContacts.Where(c => c.PatientId == patientId).ToListAsync(ct);

        // أول جهة اتصال بتبقى Primary تلقائي
        var makePrimary = dto.IsPrimary || existing.Count == 0;
        if (makePrimary) foreach (var c in existing) c.IsPrimary = false;

        var contact = new EmergencyContact
        {
            PatientId = patientId,
            FullName = dto.FullName.Trim(),
            Relationship = dto.Relationship?.Trim(),
            PhoneNumber = dto.PhoneNumber,
            IsPrimary = makePrimary
        };
        _db.EmergencyContacts.Add(contact);
        await _db.SaveChangesAsync(ct);

        return MapContact(contact);
    }

    public async Task<EmergencyContactDto?> UpdateEmergencyContactAsync(int patientId, int emergencyContactId, SaveEmergencyContactDto dto, CancellationToken ct = default)
    {
        ValidationHelper.Validate(dto);

        var contacts = await _db.EmergencyContacts.Where(c => c.PatientId == patientId).ToListAsync(ct);
        var contact = contacts.FirstOrDefault(c => c.EmergencyContactId == emergencyContactId);
        if (contact is null) return null;

        if (dto.IsPrimary && !contact.IsPrimary)
            foreach (var c in contacts) c.IsPrimary = false;

        contact.FullName = dto.FullName.Trim();
        contact.Relationship = dto.Relationship?.Trim();
        contact.PhoneNumber = dto.PhoneNumber;
        // مينفعش نشيل الـ Primary من غير ما نحدد بديل (استخدم SetPrimary على جهة تانية)
        contact.IsPrimary = contact.IsPrimary || dto.IsPrimary;

        await _db.SaveChangesAsync(ct);
        return MapContact(contact);
    }

    public async Task<bool> RemoveEmergencyContactAsync(int patientId, int emergencyContactId, CancellationToken ct = default)
    {
        var contacts = await _db.EmergencyContacts.Where(c => c.PatientId == patientId).ToListAsync(ct);
        var contact = contacts.FirstOrDefault(c => c.EmergencyContactId == emergencyContactId);
        if (contact is null) return false;

        _db.EmergencyContacts.Remove(contact);

        // لو اللي اتمسح كان Primary، أول جهة متبقية بتبقى Primary
        if (contact.IsPrimary)
        {
            var next = contacts.FirstOrDefault(c => c.EmergencyContactId != emergencyContactId);
            if (next is not null) next.IsPrimary = true;
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SetPrimaryEmergencyContactAsync(int patientId, int emergencyContactId, CancellationToken ct = default)
    {
        var contacts = await _db.EmergencyContacts.Where(c => c.PatientId == patientId).ToListAsync(ct);
        var contact = contacts.FirstOrDefault(c => c.EmergencyContactId == emergencyContactId);
        if (contact is null) return false;

        foreach (var c in contacts) c.IsPrimary = false;
        contact.IsPrimary = true;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    // =====================================================
    // Helpers
    // =====================================================

    private async Task EnsurePatientExistsAsync(int patientId, CancellationToken ct)
    {
        if (!await _db.Patients.AnyAsync(p => p.PatientId == patientId, ct))
            throw new KeyNotFoundException($"Patient {patientId} was not found.");
    }

    private static MedicalProfileDto MapProfile(MedicalProfile m) => new()
    {
        MedicalProfileId = m.MedicalProfileId,
        PatientId = m.PatientId,
        BloodType = m.BloodType,
        PrimaryConditions = m.PrimaryConditions,
        CurrentNotes = m.CurrentNotes,
        LastUpdatedAt = m.LastUpdatedAt
    };

    private static PatientChronicDiseaseDto MapDisease(PatientChronicDisease l, ChronicDisease c) => new()
    {
        ChronicDiseaseId = c.ChronicDiseaseId, Name = c.Name, Description = c.Description,
        DiagnosedAt = l.DiagnosedAt, Notes = l.Notes
    };

    private static PatientAllergyDto MapAllergy(PatientAllergy l, Allergy a) => new()
    {
        AllergyId = a.AllergyId, Name = a.Name, Description = a.Description,
        Severity = l.Severity, Reaction = l.Reaction
    };

    private static PatientMedicationDto MapMedication(PatientMedication l, Medication m) => new()
    {
        MedicationId = m.MedicationId, Name = m.Name, Description = m.Description,
        Dosage = l.Dosage, Frequency = l.Frequency, StartDate = l.StartDate, EndDate = l.EndDate
    };

    private static EmergencyContactDto MapContact(EmergencyContact c) => new()
    {
        EmergencyContactId = c.EmergencyContactId, PatientId = c.PatientId, FullName = c.FullName,
        Relationship = c.Relationship, PhoneNumber = c.PhoneNumber, IsPrimary = c.IsPrimary
    };
}
