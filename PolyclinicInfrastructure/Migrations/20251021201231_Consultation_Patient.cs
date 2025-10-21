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
                name: "FK_Employees_Nursing_NursingId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_Employees_BossId",
                table: "Warehouse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalMedicalPosts",
                table: "ExternalMedicalPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_NursingId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "NursingId",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "ExternalMedicalPosts",
                newName: "ExternalMedicalPost");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Employee");

            migrationBuilder.AlterColumn<Guid>(
                name: "BossId",
                table: "Warehouse",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BossId",
                table: "Nursing",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalMedicalPost",
                table: "ExternalMedicalPost",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Boss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boss_Employee_Id",
                        column: x => x.Id,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nurse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NursingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nurse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nurse_Employee_Id",
                        column: x => x.Id,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nurse_Nursing_NursingId",
                        column: x => x.NursingId,
                        principalTable: "Nursing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Contact = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                    table.CheckConstraint("CK_Patient_Age", "Age >= 0 AND Age < 130");
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BossId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_Boss_BossId",
                        column: x => x.BossId,
                        principalTable: "Boss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Consultation Derivation",
                columns: table => new
                {
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeDer = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateTimeCDer = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentFromId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    BossId = table.Column<Guid>(type: "uuid", nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PatientId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultation Derivation", x => new { x.DoctorId, x.DepartmentToId, x.PatientId, x.DateTimeDer, x.DateTimeCDer, x.DepartmentFromId });
                    table.ForeignKey(
                        name: "FK_Consultation Derivation_Department_DepartmentFromId",
                        column: x => x.DepartmentFromId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation Derivation_Department_DepartmentToId",
                        column: x => x.DepartmentToId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation Derivation_Employee_BossId",
                        column: x => x.BossId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Consultation Derivation_Employee_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Consultation Derivation_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation Derivation_Patient_PatientId1",
                        column: x => x.PatientId1,
                        principalTable: "Patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Consultation Referral",
                columns: table => new
                {
                    ExternalMedicalPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeRem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateTimeCRem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    BossId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultation Referral", x => new { x.DoctorId, x.ExternalMedicalPostId, x.PatientId, x.DateTimeRem, x.DateTimeCRem, x.DepartmentToId, x.Diagnosis });
                    table.ForeignKey(
                        name: "FK_Consultation Referral_Department_DepartmentToId",
                        column: x => x.DepartmentToId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation Referral_Employee_BossId",
                        column: x => x.BossId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Consultation Referral_Employee_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Consultation Referral_ExternalMedicalPost_ExternalMedicalPo~",
                        column: x => x.ExternalMedicalPostId,
                        principalTable: "ExternalMedicalPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation Referral_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation Referral_Patient_PatientId1",
                        column: x => x.PatientId1,
                        principalTable: "Patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Derivation",
                columns: table => new
                {
                    DepartmentFromId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeDer = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Derivation", x => new { x.DepartmentFromId, x.PatientId, x.DateTimeDer });
                    table.ForeignKey(
                        name: "FK_Derivation_Department_DepartmentFromId",
                        column: x => x.DepartmentFromId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Derivation_Department_DepartmentToId",
                        column: x => x.DepartmentToId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Derivation_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Derivation_Patient_PatientId1",
                        column: x => x.PatientId1,
                        principalTable: "Patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MedicalStaff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalStaff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalStaff_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalStaff_Employee_Id",
                        column: x => x.Id,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicationRequest",
                columns: table => new
                {
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationRequest", x => new { x.MedicationId, x.DepartmentId, x.RequestDate });
                    table.ForeignKey(
                        name: "FK_MedicationRequest_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MedicationRequest_Medicines_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medicines",
                        principalColumn: "IdMed",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Referral",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeRem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalMedicalPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referral", x => new { x.ExternalMedicalPostId, x.PatientId, x.DateTimeRem });
                    table.ForeignKey(
                        name: "FK_Referral_Department_DepartmentToId",
                        column: x => x.DepartmentToId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referral_ExternalMedicalPost_ExternalMedicalPostId",
                        column: x => x.ExternalMedicalPostId,
                        principalTable: "ExternalMedicalPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referral_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referral_Patient_PatientId1",
                        column: x => x.PatientId1,
                        principalTable: "Patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseRequest",
                columns: table => new
                {
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BossId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseRequest", x => new { x.WarehouseId, x.DepartmentId, x.RequestDate });
                    table.ForeignKey(
                        name: "FK_WarehouseRequest_Boss_BossId",
                        column: x => x.BossId,
                        principalTable: "Boss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseRequest_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WarehouseRequest_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Doctor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctor_MedicalStaff_Id",
                        column: x => x.Id,
                        principalTable: "MedicalStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyRoomCare",
                columns: table => new
                {
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CareDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuardDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyRoomCare", x => new { x.DoctorId, x.PatientId, x.CareDate, x.GuardDate });
                    table.ForeignKey(
                        name: "FK_EmergencyRoomCare_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmergencyRoomCare_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nursing_BossId",
                table: "Nursing",
                column: "BossId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Derivation_BossId",
                table: "Consultation Derivation",
                column: "BossId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Derivation_DepartmentFromId",
                table: "Consultation Derivation",
                column: "DepartmentFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Derivation_DepartmentToId",
                table: "Consultation Derivation",
                column: "DepartmentToId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Derivation_PatientId",
                table: "Consultation Derivation",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Derivation_PatientId1",
                table: "Consultation Derivation",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Referral_BossId",
                table: "Consultation Referral",
                column: "BossId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Referral_DepartmentToId",
                table: "Consultation Referral",
                column: "DepartmentToId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Referral_ExternalMedicalPostId",
                table: "Consultation Referral",
                column: "ExternalMedicalPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Referral_PatientId",
                table: "Consultation Referral",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation Referral_PatientId1",
                table: "Consultation Referral",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Department_BossId",
                table: "Department",
                column: "BossId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Derivation_DepartmentToId",
                table: "Derivation",
                column: "DepartmentToId");

            migrationBuilder.CreateIndex(
                name: "IX_Derivation_PatientId",
                table: "Derivation",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Derivation_PatientId1",
                table: "Derivation",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoomCare_PatientId",
                table: "EmergencyRoomCare",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalStaff_DepartmentId",
                table: "MedicalStaff",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRequest_DepartmentId",
                table: "MedicationRequest",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Nurse_NursingId",
                table: "Nurse",
                column: "NursingId");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Contact",
                table: "Patient",
                column: "Contact",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Referral_DepartmentToId",
                table: "Referral",
                column: "DepartmentToId");

            migrationBuilder.CreateIndex(
                name: "IX_Referral_PatientId",
                table: "Referral",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Referral_PatientId1",
                table: "Referral",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_BossId",
                table: "WarehouseRequest",
                column: "BossId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_DepartmentId",
                table: "WarehouseRequest",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nursing_Boss_BossId",
                table: "Nursing",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_Boss_BossId",
                table: "Warehouse",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nursing_Boss_BossId",
                table: "Nursing");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_Boss_BossId",
                table: "Warehouse");

            migrationBuilder.DropTable(
                name: "Consultation Derivation");

            migrationBuilder.DropTable(
                name: "Consultation Referral");

            migrationBuilder.DropTable(
                name: "Derivation");

            migrationBuilder.DropTable(
                name: "EmergencyRoomCare");

            migrationBuilder.DropTable(
                name: "MedicationRequest");

            migrationBuilder.DropTable(
                name: "Nurse");

            migrationBuilder.DropTable(
                name: "Referral");

            migrationBuilder.DropTable(
                name: "WarehouseRequest");

            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "MedicalStaff");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Boss");

            migrationBuilder.DropIndex(
                name: "IX_Nursing_BossId",
                table: "Nursing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalMedicalPost",
                table: "ExternalMedicalPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "BossId",
                table: "Nursing");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "ExternalMedicalPost",
                newName: "ExternalMedicalPosts");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Employees");

            migrationBuilder.AlterColumn<Guid>(
                name: "BossId",
                table: "Warehouse",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Employees",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "NursingId",
                table: "Employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalMedicalPosts",
                table: "ExternalMedicalPosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NursingId",
                table: "Employees",
                column: "NursingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Nursing_NursingId",
                table: "Employees",
                column: "NursingId",
                principalTable: "Nursing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_Employees_BossId",
                table: "Warehouse",
                column: "BossId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
