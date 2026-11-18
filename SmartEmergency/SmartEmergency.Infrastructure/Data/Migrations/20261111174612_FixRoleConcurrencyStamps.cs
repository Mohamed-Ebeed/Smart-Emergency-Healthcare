using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartEmergency.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRoleConcurrencyStamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0001-4a00-9000-000000000001",
                column: "ConcurrencyStamp",
                value: "role-patient-stamp");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0002-4a00-9000-000000000002",
                column: "ConcurrencyStamp",
                value: "role-operator-stamp");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0003-4a00-9000-000000000003",
                column: "ConcurrencyStamp",
                value: "role-hospital-staff-stamp");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0004-4a00-9000-000000000004",
                column: "ConcurrencyStamp",
                value: "role-paramedic-stamp");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0005-4a00-9000-000000000005",
                column: "ConcurrencyStamp",
                value: "role-doctor-stamp");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0006-4a00-9000-000000000006",
                column: "ConcurrencyStamp",
                value: "role-admin-stamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0001-4a00-9000-000000000001",
                column: "ConcurrencyStamp",
                value: "47a0e9fc-1eac-4a4e-9e0e-bfb17933b0b9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0002-4a00-9000-000000000002",
                column: "ConcurrencyStamp",
                value: "bc5c0800-a540-4349-b2b4-15522314f3a9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0003-4a00-9000-000000000003",
                column: "ConcurrencyStamp",
                value: "d9bc4763-6f13-4afb-9271-ccbf51d2fa7c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0004-4a00-9000-000000000004",
                column: "ConcurrencyStamp",
                value: "4070f98f-5810-43a2-9880-f14632d5c696");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0005-4a00-9000-000000000005",
                column: "ConcurrencyStamp",
                value: "80766926-ef04-4dfc-8fb4-34517dc579c0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1f0c9a0-0006-4a00-9000-000000000006",
                column: "ConcurrencyStamp",
                value: "7230b495-1924-4273-863f-ed6ff3e3fe63");
        }
    }
}
