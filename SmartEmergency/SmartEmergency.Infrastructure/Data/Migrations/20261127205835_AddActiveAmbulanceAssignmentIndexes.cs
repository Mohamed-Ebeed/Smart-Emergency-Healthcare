using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartEmergency.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveAmbulanceAssignmentIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AmbulanceAssignments_AmbulanceId",
                table: "AmbulanceAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AmbulanceAssignments_EmergencyRequestId",
                table: "AmbulanceAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AmbulanceAssignments_ParamedicId",
                table: "AmbulanceAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_AmbulanceAssignments_AmbulanceId",
                table: "AmbulanceAssignments",
                column: "AmbulanceId",
                unique: true,
                filter: "[CompletedAt] IS NULL AND [AssignmentStatus] = 'Accepted'");

            migrationBuilder.CreateIndex(
                name: "IX_AmbulanceAssignments_EmergencyRequestId",
                table: "AmbulanceAssignments",
                column: "EmergencyRequestId",
                unique: true,
                filter: "[CompletedAt] IS NULL AND [AssignmentStatus] = 'Accepted'");

            migrationBuilder.CreateIndex(
                name: "IX_AmbulanceAssignments_ParamedicId",
                table: "AmbulanceAssignments",
                column: "ParamedicId",
                unique: true,
                filter: "[CompletedAt] IS NULL AND [AssignmentStatus] = 'Accepted'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AmbulanceAssignments_AmbulanceId",
                table: "AmbulanceAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AmbulanceAssignments_EmergencyRequestId",
                table: "AmbulanceAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AmbulanceAssignments_ParamedicId",
                table: "AmbulanceAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_AmbulanceAssignments_AmbulanceId",
                table: "AmbulanceAssignments",
                column: "AmbulanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AmbulanceAssignments_EmergencyRequestId",
                table: "AmbulanceAssignments",
                column: "EmergencyRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AmbulanceAssignments_ParamedicId",
                table: "AmbulanceAssignments",
                column: "ParamedicId");
        }
    }
}
