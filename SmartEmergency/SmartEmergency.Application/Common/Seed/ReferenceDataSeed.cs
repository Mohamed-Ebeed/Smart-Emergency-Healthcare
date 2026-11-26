using SmartEmergency.Domain.Entities;
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Application.Common.Seed;

// Fixed lists to be seeded with HasData in the DbContext.
// Departments are NOT here, they are created per hospital.
public static class ReferenceDataSeed
{
    public static readonly List<EmergencyType> EmergencyTypes = new()
    {
        new EmergencyType { EmergencyTypeId = 1, Name = "Cardiac Arrest", Description = "Heart stopped or not beating normally", DefaultPriority = Priority.Critical },
        new EmergencyType { EmergencyTypeId = 2, Name = "Heart Attack", Description = "Chest pain, possible heart attack", DefaultPriority = Priority.Critical },
        new EmergencyType { EmergencyTypeId = 3, Name = "Stroke", Description = "Face drooping, arm weakness, speech problems", DefaultPriority = Priority.Critical },
        new EmergencyType { EmergencyTypeId = 4, Name = "Breathing Difficulty", Description = "Shortness of breath or choking", DefaultPriority = Priority.Critical },
        new EmergencyType { EmergencyTypeId = 5, Name = "Severe Bleeding", Description = "Heavy bleeding that doesn't stop", DefaultPriority = Priority.Critical },
        new EmergencyType { EmergencyTypeId = 6, Name = "Unconscious", Description = "Person not responding", DefaultPriority = Priority.Critical },
        new EmergencyType { EmergencyTypeId = 7, Name = "Road Accident", Description = "Car or motorcycle accident", DefaultPriority = Priority.High },
        new EmergencyType { EmergencyTypeId = 8, Name = "Burns", Description = "Fire, hot liquid or chemical burns", DefaultPriority = Priority.High },
        new EmergencyType { EmergencyTypeId = 9, Name = "Seizure", Description = "Convulsions or fits", DefaultPriority = Priority.High },
        new EmergencyType { EmergencyTypeId = 10, Name = "Poisoning", Description = "Swallowed something toxic or overdose", DefaultPriority = Priority.High },
        new EmergencyType { EmergencyTypeId = 11, Name = "Severe Allergic Reaction", Description = "Swelling, rash and trouble breathing", DefaultPriority = Priority.High },
        new EmergencyType { EmergencyTypeId = 12, Name = "Pregnancy Emergency", Description = "Labor or pregnancy complications", DefaultPriority = Priority.High },
        new EmergencyType { EmergencyTypeId = 13, Name = "Diabetic Emergency", Description = "Very high or very low blood sugar", DefaultPriority = Priority.Medium },
        new EmergencyType { EmergencyTypeId = 14, Name = "Fracture", Description = "Broken bone or bad fall", DefaultPriority = Priority.Medium },
        new EmergencyType { EmergencyTypeId = 15, Name = "High Fever", Description = "Very high temperature", DefaultPriority = Priority.Low },
        new EmergencyType { EmergencyTypeId = 16, Name = "Other", Description = "Anything else", DefaultPriority = Priority.Medium }
    };

    public static readonly List<ChronicDisease> ChronicDiseases = new()
    {
        new ChronicDisease { ChronicDiseaseId = 1, Name = "Diabetes Type 1" },
        new ChronicDisease { ChronicDiseaseId = 2, Name = "Diabetes Type 2" },
        new ChronicDisease { ChronicDiseaseId = 3, Name = "Hypertension" },
        new ChronicDisease { ChronicDiseaseId = 4, Name = "Asthma" },
        new ChronicDisease { ChronicDiseaseId = 5, Name = "Heart Disease" },
        new ChronicDisease { ChronicDiseaseId = 6, Name = "Chronic Kidney Disease" },
        new ChronicDisease { ChronicDiseaseId = 7, Name = "COPD" },
        new ChronicDisease { ChronicDiseaseId = 8, Name = "Epilepsy" },
        new ChronicDisease { ChronicDiseaseId = 9, Name = "Hepatitis C" },
        new ChronicDisease { ChronicDiseaseId = 10, Name = "Thyroid Disease" },
        new ChronicDisease { ChronicDiseaseId = 11, Name = "Anemia" },
        new ChronicDisease { ChronicDiseaseId = 12, Name = "Cancer" }
    };

    public static readonly List<Allergy> Allergies = new()
    {
        new Allergy { AllergyId = 1, Name = "Penicillin" },
        new Allergy { AllergyId = 2, Name = "Aspirin" },
        new Allergy { AllergyId = 3, Name = "Ibuprofen" },
        new Allergy { AllergyId = 4, Name = "Sulfa Drugs" },
        new Allergy { AllergyId = 5, Name = "Latex" },
        new Allergy { AllergyId = 6, Name = "Peanuts" },
        new Allergy { AllergyId = 7, Name = "Seafood" },
        new Allergy { AllergyId = 8, Name = "Eggs" },
        new Allergy { AllergyId = 9, Name = "Milk" },
        new Allergy { AllergyId = 10, Name = "Bee Stings" },
        new Allergy { AllergyId = 11, Name = "Dust" },
        new Allergy { AllergyId = 12, Name = "Pollen" }
    };

    public static readonly List<Medication> Medications = new()
    {
        new Medication { MedicationId = 1, Name = "Insulin" },
        new Medication { MedicationId = 2, Name = "Metformin" },
        new Medication { MedicationId = 3, Name = "Amlodipine" },
        new Medication { MedicationId = 4, Name = "Lisinopril" },
        new Medication { MedicationId = 5, Name = "Aspirin" },
        new Medication { MedicationId = 6, Name = "Warfarin" },
        new Medication { MedicationId = 7, Name = "Clopidogrel" },
        new Medication { MedicationId = 8, Name = "Atorvastatin" },
        new Medication { MedicationId = 9, Name = "Salbutamol Inhaler" },
        new Medication { MedicationId = 10, Name = "Levothyroxine" },
        new Medication { MedicationId = 11, Name = "Paracetamol" },
        new Medication { MedicationId = 12, Name = "Omeprazole" }
    };
}