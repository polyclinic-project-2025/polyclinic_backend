using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConsultationPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Derivation_Employee_BossId",
                table: "Consultation Derivation");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Derivation_Employee_DoctorId",
                table: "Consultation Derivation");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Referral_Employee_BossId",
                table: "Consultation Referral");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Referral_Employee_DoctorId",
                table: "Consultation Referral");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Patient",
                newName: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Derivation_Boss_BossId",
                table: "Consultation Derivation",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Derivation_Doctor_DoctorId",
                table: "Consultation Derivation",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Referral_Boss_BossId",
                table: "Consultation Referral",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Referral_Doctor_DoctorId",
                table: "Consultation Referral",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Derivation_Boss_BossId",
                table: "Consultation Derivation");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Derivation_Doctor_DoctorId",
                table: "Consultation Derivation");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Referral_Boss_BossId",
                table: "Consultation Referral");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Referral_Doctor_DoctorId",
                table: "Consultation Referral");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Patient",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Derivation_Employee_BossId",
                table: "Consultation Derivation",
                column: "BossId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Derivation_Employee_DoctorId",
                table: "Consultation Derivation",
                column: "DoctorId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Referral_Employee_BossId",
                table: "Consultation Referral",
                column: "BossId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Referral_Employee_DoctorId",
                table: "Consultation Referral",
                column: "DoctorId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
