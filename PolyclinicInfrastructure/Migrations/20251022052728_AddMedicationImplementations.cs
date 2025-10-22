using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationImplementations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Medicine_Quantities_NonNegative",
                table: "Medications");

            migrationBuilder.CreateTable(
                name: "MedicationDerivations",
                columns: table => new
                {
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeDer = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateTimeCDer = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentFromId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdMed = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDerivations", x => new { x.DepartmentToId, x.DepartmentFromId, x.PatientId, x.DateTimeDer, x.DateTimeCDer, x.DoctorId, x.IdMed });
                    table.ForeignKey(
                        name: "FK_MedicationDerivations_Consultation Derivation_DepartmentToI~",
                        columns: x => new { x.DepartmentToId, x.DepartmentFromId, x.PatientId, x.DateTimeDer, x.DateTimeCDer, x.DoctorId },
                        principalTable: "Consultation Derivation",
                        principalColumns: new[] { "DoctorId", "DepartmentToId", "PatientId", "DateTimeDer", "DateTimeCDer", "DepartmentFromId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationDerivations_Medications_IdMed",
                        column: x => x.IdMed,
                        principalTable: "Medications",
                        principalColumn: "IdMed",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicationEmergency",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CareDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuardDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IdMed = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationEmergency", x => new { x.DoctorId, x.PatientId, x.CareDate, x.GuardDate, x.IdMed });
                    table.ForeignKey(
                        name: "FK_MedicationEmergency_EmergencyRoomCare_DoctorId_PatientId_Ca~",
                        columns: x => new { x.DoctorId, x.PatientId, x.CareDate, x.GuardDate },
                        principalTable: "EmergencyRoomCare",
                        principalColumns: new[] { "DoctorId", "PatientId", "CareDate", "GuardDate" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationEmergency_Medications_IdMed",
                        column: x => x.IdMed,
                        principalTable: "Medications",
                        principalColumn: "IdMed",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicationReferrals",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalMedicalPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeRem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateTimeCRem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(1000)", nullable: false),
                    IdMed = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationReferrals", x => new { x.DoctorId, x.ExternalMedicalPostId, x.PatientId, x.DateTimeRem, x.DateTimeCRem, x.DepartmentToId, x.Diagnosis, x.IdMed });
                    table.ForeignKey(
                        name: "FK_MedicationReferrals_Consultation Referral_DoctorId_External~",
                        columns: x => new { x.DoctorId, x.ExternalMedicalPostId, x.PatientId, x.DateTimeRem, x.DateTimeCRem, x.DepartmentToId, x.Diagnosis },
                        principalTable: "Consultation Referral",
                        principalColumns: new[] { "DoctorId", "ExternalMedicalPostId", "PatientId", "DateTimeRem", "DateTimeCRem", "DepartmentToId", "Diagnosis" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationReferrals_Medications_IdMed",
                        column: x => x.IdMed,
                        principalTable: "Medications",
                        principalColumn: "IdMed",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockDepartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdMed = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDepartments", x => new { x.Id, x.IdMed });
                    table.ForeignKey(
                        name: "FK_StockDepartments_Department_Id",
                        column: x => x.Id,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockDepartments_Medications_IdMed",
                        column: x => x.IdMed,
                        principalTable: "Medications",
                        principalColumn: "IdMed",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDerivations_IdMed",
                table: "MedicationDerivations",
                column: "IdMed");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationEmergency_IdMed",
                table: "MedicationEmergency",
                column: "IdMed");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReferrals_IdMed",
                table: "MedicationReferrals",
                column: "IdMed");

            migrationBuilder.CreateIndex(
                name: "IX_StockDepartments_IdMed",
                table: "StockDepartments",
                column: "IdMed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicationDerivations");

            migrationBuilder.DropTable(
                name: "MedicationEmergency");

            migrationBuilder.DropTable(
                name: "MedicationReferrals");

            migrationBuilder.DropTable(
                name: "StockDepartments");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Medicine_Quantities_NonNegative",
                table: "Medications",
                sql: "\"QuantityA\" >= 0 AND \"QuantityNurse\" >= 0");
        }
    }
}
