using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AnotherOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRoomCare_Doctor_DoctorId",
                table: "EmergencyRoomCare");

            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRoomCare_EmergencyRoom_DoctorId_GuardDate",
                table: "EmergencyRoomCare");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationDerivations_Consultation Derivation_DepartmentToI~",
                table: "MedicationDerivations");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationEmergency_EmergencyRoomCare_DoctorId_PatientId_Ca~",
                table: "MedicationEmergency");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationReferrals_Consultation Referral_DoctorId_External~",
                table: "MedicationReferrals");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationRequest_Department_DepartmentId",
                table: "MedicationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseRequest_Department_DepartmentId",
                table: "WarehouseRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WarehouseRequest",
                table: "WarehouseRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationRequest",
                table: "MedicationRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationEmergency",
                table: "MedicationEmergency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmergencyRoomCare",
                table: "EmergencyRoomCare");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyRoomCare_DoctorId_GuardDate",
                table: "EmergencyRoomCare");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmergencyRoom",
                table: "EmergencyRoom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consultation Referral",
                table: "Consultation Referral");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consultation Derivation",
                table: "Consultation Derivation");

            migrationBuilder.DropColumn(
                name: "GuardDate",
                table: "EmergencyRoomCare");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "EmergencyRoomCare",
                newName: "EmergencyRoomId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "WarehouseRequest",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "WarehouseRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Identification",
                table: "Patient",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "MedicationRequest",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "MedicationReferrals",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)");

            migrationBuilder.AddColumn<Guid>(
                name: "EmergencyRoomCareId",
                table: "MedicationEmergency",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Identification",
                table: "Employee",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "EmergencyRoomCare",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "EmergencyRoom",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "Consultation Referral",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WarehouseRequest",
                table: "WarehouseRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationRequest",
                table: "MedicationRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationEmergency",
                table: "MedicationEmergency",
                columns: new[] { "EmergencyRoomCareId", "IdMed" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmergencyRoomCare",
                table: "EmergencyRoomCare",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmergencyRoom",
                table: "EmergencyRoom",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consultation Referral",
                table: "Consultation Referral",
                columns: new[] { "DoctorId", "ExternalMedicalPostId", "PatientId", "DateTimeRem", "DateTimeCRem" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consultation Derivation",
                table: "Consultation Derivation",
                columns: new[] { "DoctorId", "PatientId", "DateTimeDer", "DateTimeCDer", "DepartmentFromId" });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_WarehouseId_DepartmentId_RequestDate",
                table: "WarehouseRequest",
                columns: new[] { "WarehouseId", "DepartmentId", "RequestDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRequest_MedicationId_DepartmentId_RequestDate",
                table: "MedicationRequest",
                columns: new[] { "MedicationId", "DepartmentId", "RequestDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDerivations_DoctorId_PatientId_DateTimeDer_DateTi~",
                table: "MedicationDerivations",
                columns: new[] { "DoctorId", "PatientId", "DateTimeDer", "DateTimeCDer", "DepartmentFromId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoomCare_EmergencyRoomId",
                table: "EmergencyRoomCare",
                column: "EmergencyRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoom_DoctorId_GuardDate",
                table: "EmergencyRoom",
                columns: new[] { "DoctorId", "GuardDate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRoomCare_EmergencyRoom_EmergencyRoomId",
                table: "EmergencyRoomCare",
                column: "EmergencyRoomId",
                principalTable: "EmergencyRoom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationDerivations_Consultation Derivation_DoctorId_Pati~",
                table: "MedicationDerivations",
                columns: new[] { "DoctorId", "PatientId", "DateTimeDer", "DateTimeCDer", "DepartmentFromId" },
                principalTable: "Consultation Derivation",
                principalColumns: new[] { "DoctorId", "PatientId", "DateTimeDer", "DateTimeCDer", "DepartmentFromId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationEmergency_EmergencyRoomCare_EmergencyRoomCareId",
                table: "MedicationEmergency",
                column: "EmergencyRoomCareId",
                principalTable: "EmergencyRoomCare",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationReferrals_Consultation Referral_DoctorId_External~",
                table: "MedicationReferrals",
                columns: new[] { "DoctorId", "ExternalMedicalPostId", "PatientId", "DateTimeRem", "DateTimeCRem" },
                principalTable: "Consultation Referral",
                principalColumns: new[] { "DoctorId", "ExternalMedicalPostId", "PatientId", "DateTimeRem", "DateTimeCRem" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationRequest_Department_DepartmentId",
                table: "MedicationRequest",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseRequest_Department_DepartmentId",
                table: "WarehouseRequest",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRoomCare_EmergencyRoom_EmergencyRoomId",
                table: "EmergencyRoomCare");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationDerivations_Consultation Derivation_DoctorId_Pati~",
                table: "MedicationDerivations");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationEmergency_EmergencyRoomCare_EmergencyRoomCareId",
                table: "MedicationEmergency");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationReferrals_Consultation Referral_DoctorId_External~",
                table: "MedicationReferrals");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationRequest_Department_DepartmentId",
                table: "MedicationRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseRequest_Department_DepartmentId",
                table: "WarehouseRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WarehouseRequest",
                table: "WarehouseRequest");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseRequest_WarehouseId_DepartmentId_RequestDate",
                table: "WarehouseRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationRequest",
                table: "MedicationRequest");

            migrationBuilder.DropIndex(
                name: "IX_MedicationRequest_MedicationId_DepartmentId_RequestDate",
                table: "MedicationRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationEmergency",
                table: "MedicationEmergency");

            migrationBuilder.DropIndex(
                name: "IX_MedicationDerivations_DoctorId_PatientId_DateTimeDer_DateTi~",
                table: "MedicationDerivations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmergencyRoomCare",
                table: "EmergencyRoomCare");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyRoomCare_EmergencyRoomId",
                table: "EmergencyRoomCare");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmergencyRoom",
                table: "EmergencyRoom");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyRoom_DoctorId_GuardDate",
                table: "EmergencyRoom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consultation Referral",
                table: "Consultation Referral");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consultation Derivation",
                table: "Consultation Derivation");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WarehouseRequest");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MedicationRequest");

            migrationBuilder.DropColumn(
                name: "EmergencyRoomCareId",
                table: "MedicationEmergency");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmergencyRoomCare");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmergencyRoom");

            migrationBuilder.RenameColumn(
                name: "EmergencyRoomId",
                table: "EmergencyRoomCare",
                newName: "DoctorId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "WarehouseRequest",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Identification",
                table: "Patient",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "MedicationReferrals",
                type: "character varying(1000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Identification",
                table: "Employee",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateOnly>(
                name: "GuardDate",
                table: "EmergencyRoomCare",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "Consultation Referral",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WarehouseRequest",
                table: "WarehouseRequest",
                columns: new[] { "WarehouseId", "DepartmentId", "RequestDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationRequest",
                table: "MedicationRequest",
                columns: new[] { "MedicationId", "DepartmentId", "RequestDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationEmergency",
                table: "MedicationEmergency",
                columns: new[] { "DoctorId", "PatientId", "CareDate", "GuardDate", "IdMed" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmergencyRoomCare",
                table: "EmergencyRoomCare",
                columns: new[] { "DoctorId", "PatientId", "CareDate", "GuardDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmergencyRoom",
                table: "EmergencyRoom",
                columns: new[] { "DoctorId", "GuardDate" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consultation Referral",
                table: "Consultation Referral",
                columns: new[] { "DoctorId", "ExternalMedicalPostId", "PatientId", "DateTimeRem", "DateTimeCRem", "DepartmentToId", "Diagnosis" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consultation Derivation",
                table: "Consultation Derivation",
                columns: new[] { "DoctorId", "DepartmentToId", "PatientId", "DateTimeDer", "DateTimeCDer", "DepartmentFromId" });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoomCare_DoctorId_GuardDate",
                table: "EmergencyRoomCare",
                columns: new[] { "DoctorId", "GuardDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRoomCare_Doctor_DoctorId",
                table: "EmergencyRoomCare",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRoomCare_EmergencyRoom_DoctorId_GuardDate",
                table: "EmergencyRoomCare",
                columns: new[] { "DoctorId", "GuardDate" },
                principalTable: "EmergencyRoom",
                principalColumns: new[] { "DoctorId", "GuardDate" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationDerivations_Consultation Derivation_DepartmentToI~",
                table: "MedicationDerivations",
                columns: new[] { "DepartmentToId", "DepartmentFromId", "PatientId", "DateTimeDer", "DateTimeCDer", "DoctorId" },
                principalTable: "Consultation Derivation",
                principalColumns: new[] { "DoctorId", "DepartmentToId", "PatientId", "DateTimeDer", "DateTimeCDer", "DepartmentFromId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationEmergency_EmergencyRoomCare_DoctorId_PatientId_Ca~",
                table: "MedicationEmergency",
                columns: new[] { "DoctorId", "PatientId", "CareDate", "GuardDate" },
                principalTable: "EmergencyRoomCare",
                principalColumns: new[] { "DoctorId", "PatientId", "CareDate", "GuardDate" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationReferrals_Consultation Referral_DoctorId_External~",
                table: "MedicationReferrals",
                columns: new[] { "DoctorId", "ExternalMedicalPostId", "PatientId", "DateTimeRem", "DateTimeCRem", "DepartmentToId", "Diagnosis" },
                principalTable: "Consultation Referral",
                principalColumns: new[] { "DoctorId", "ExternalMedicalPostId", "PatientId", "DateTimeRem", "DateTimeCRem", "DepartmentToId", "Diagnosis" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationRequest_Department_DepartmentId",
                table: "MedicationRequest",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseRequest_Department_DepartmentId",
                table: "WarehouseRequest",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
