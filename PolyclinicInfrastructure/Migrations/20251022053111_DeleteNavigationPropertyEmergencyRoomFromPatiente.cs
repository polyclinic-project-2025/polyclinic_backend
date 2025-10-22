using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteNavigationPropertyEmergencyRoomFromPatiente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRoom_Patient_PatientId",
                table: "EmergencyRoom");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyRoom_PatientId",
                table: "EmergencyRoom");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "EmergencyRoom");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "EmergencyRoom",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoom_PatientId",
                table: "EmergencyRoom",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRoom_Patient_PatientId",
                table: "EmergencyRoom",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "PatientId");
        }
    }
}
