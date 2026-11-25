using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartEmergency.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Medications",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Allergies",
                columns: new[] { "AllergyId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Penicillin" },
                    { 2, null, "Aspirin" },
                    { 3, null, "Ibuprofen" },
                    { 4, null, "Sulfa Drugs" },
                    { 5, null, "Latex" },
                    { 6, null, "Peanuts" },
                    { 7, null, "Seafood" },
                    { 8, null, "Eggs" },
                    { 9, null, "Milk" },
                    { 10, null, "Bee Stings" },
                    { 11, null, "Dust" },
                    { 12, null, "Pollen" }
                });

            migrationBuilder.InsertData(
                table: "ChronicDiseases",
                columns: new[] { "ChronicDiseaseId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Diabetes Type 1" },
                    { 2, null, "Diabetes Type 2" },
                    { 3, null, "Hypertension" },
                    { 4, null, "Asthma" },
                    { 5, null, "Heart Disease" },
                    { 6, null, "Chronic Kidney Disease" },
                    { 7, null, "COPD" },
                    { 8, null, "Epilepsy" },
                    { 9, null, "Hepatitis C" },
                    { 10, null, "Thyroid Disease" },
                    { 11, null, "Anemia" },
                    { 12, null, "Cancer" }
                });

            migrationBuilder.InsertData(
                table: "EmergencyTypes",
                columns: new[] { "EmergencyTypeId", "DefaultPriority", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Critical", "Heart stopped or not beating normally", "Cardiac Arrest" },
                    { 2, "Critical", "Chest pain, possible heart attack", "Heart Attack" },
                    { 3, "Critical", "Face drooping, arm weakness, speech problems", "Stroke" },
                    { 4, "Critical", "Shortness of breath or choking", "Breathing Difficulty" },
                    { 5, "Critical", "Heavy bleeding that doesn't stop", "Severe Bleeding" },
                    { 6, "Critical", "Person not responding", "Unconscious" },
                    { 7, "High", "Car or motorcycle accident", "Road Accident" },
                    { 8, "High", "Fire, hot liquid or chemical burns", "Burns" },
                    { 9, "High", "Convulsions or fits", "Seizure" },
                    { 10, "High", "Swallowed something toxic or overdose", "Poisoning" },
                    { 11, "High", "Swelling, rash and trouble breathing", "Severe Allergic Reaction" },
                    { 12, "High", "Labor or pregnancy complications", "Pregnancy Emergency" },
                    { 13, "Medium", "Very high or very low blood sugar", "Diabetic Emergency" },
                    { 14, "Medium", "Broken bone or bad fall", "Fracture" },
                    { 15, "Low", "Very high temperature", "High Fever" },
                    { 16, "Medium", "Anything else", "Other" }
                });

            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "MedicationId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Insulin" },
                    { 2, null, "Metformin" },
                    { 3, null, "Amlodipine" },
                    { 4, null, "Lisinopril" },
                    { 5, null, "Aspirin" },
                    { 6, null, "Warfarin" },
                    { 7, null, "Clopidogrel" },
                    { 8, null, "Atorvastatin" },
                    { 9, null, "Salbutamol Inhaler" },
                    { 10, null, "Levothyroxine" },
                    { 11, null, "Paracetamol" },
                    { 12, null, "Omeprazole" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_Name",
                table: "Medications",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Medications_Name",
                table: "Medications");

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Allergies",
                keyColumn: "AllergyId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ChronicDiseases",
                keyColumn: "ChronicDiseaseId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "EmergencyTypes",
                keyColumn: "EmergencyTypeId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 12);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);
        }
    }
}
