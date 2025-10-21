using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Consultation_Patient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Derivation_Patient_PatientId1",
                table: "Consultation Derivation");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Referral_Patient_PatientId1",
                table: "Consultation Referral");

            migrationBuilder.DropForeignKey(
                name: "FK_Derivation_Patient_PatientId1",
                table: "Derivation");

            migrationBuilder.DropForeignKey(
                name: "FK_Referral_Patient_PatientId1",
                table: "Referral");

            migrationBuilder.DropIndex(
                name: "IX_Referral_PatientId1",
                table: "Referral");

            migrationBuilder.DropIndex(
                name: "IX_Derivation_PatientId1",
                table: "Derivation");

            migrationBuilder.DropIndex(
                name: "IX_Consultation Referral_PatientId1",
                table: "Consultation Referral");

            migrationBuilder.DropIndex(
                name: "IX_Consultation Derivation_PatientId1",
                table: "Consultation Derivation");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Referral");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Derivation");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Consultation Referral");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Consultation Derivation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientId1",
                table: "Referral",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId1",
                table: "Derivation",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId1",
                table: "Consultation Referral",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId1",
                table: "Consultation Derivation",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Referral_PatientId1",
                table: "Referral",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Derivation_PatientId1",
                table: "Derivation",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Referral_PatientId1",
                table: "Consultation Referral",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Derivation_PatientId1",
                table: "Consultation Derivation",
                column: "PatientId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Derivation_Patient_PatientId1",
                table: "Consultation Derivation",
                column: "PatientId1",
                principalTable: "Patient",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Referral_Patient_PatientId1",
                table: "Consultation Referral",
                column: "PatientId1",
                principalTable: "Patient",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Derivation_Patient_PatientId1",
                table: "Derivation",
                column: "PatientId1",
                principalTable: "Patient",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Referral_Patient_PatientId1",
                table: "Referral",
                column: "PatientId1",
                principalTable: "Patient",
                principalColumn: "PatientId");
        }
    }
}
