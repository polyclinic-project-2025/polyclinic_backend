using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    EmploymentStatus = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalMedicalPost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalMedicalPost", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    IdMed = table.Column<Guid>(type: "uuid", nullable: false),
                    Format = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CommercialName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CommercialCompany = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ScientificName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    QuantityA = table.Column<int>(type: "integer", nullable: false),
                    QuantityNurse = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.IdMed);
                    table.CheckConstraint("CK_Medicine_Quantities_NonNegative", "\"QuantityA\" >= 0 AND \"QuantityNurse\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Contact = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.PatientId);
                    table.CheckConstraint("CK_Patient_Age", "\"Age\" >= 0 AND \"Age\" < 130");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Email);
                });

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
                name: "Nursing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BossId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nursing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nursing_Boss_BossId",
                        column: x => x.BossId,
                        principalTable: "Boss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BossId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouse_Boss_BossId",
                        column: x => x.BossId,
                        principalTable: "Boss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Derivation",
                columns: table => new
                {
                    DepartmentFromId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeDer = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false)
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
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_MedicationRequest_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
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
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false)
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
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
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
                    Diagnosis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultation Derivation", x => new { x.DoctorId, x.DepartmentToId, x.PatientId, x.DateTimeDer, x.DateTimeCDer, x.DepartmentFromId });
                    table.ForeignKey(
                        name: "FK_Consultation Derivation_Boss_BossId",
                        column: x => x.BossId,
                        principalTable: "Boss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                        name: "FK_Consultation Derivation_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Consultation Derivation_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
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
                    BossId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultation Referral", x => new { x.DoctorId, x.ExternalMedicalPostId, x.PatientId, x.DateTimeRem, x.DateTimeCRem, x.DepartmentToId, x.Diagnosis });
                    table.ForeignKey(
                        name: "FK_Consultation Referral_Boss_BossId",
                        column: x => x.BossId,
                        principalTable: "Boss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Consultation Referral_Department_DepartmentToId",
                        column: x => x.DepartmentToId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation Referral_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
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
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Nursing_BossId",
                table: "Nursing",
                column: "BossId");

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
                name: "IX_Warehouse_BossId",
                table: "Warehouse",
                column: "BossId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_BossId",
                table: "WarehouseRequest",
                column: "BossId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_DepartmentId",
                table: "WarehouseRequest",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "User");

            migrationBuilder.DropTable(
                name: "WarehouseRequest");

            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "Nursing");

            migrationBuilder.DropTable(
                name: "ExternalMedicalPost");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "MedicalStaff");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Boss");

            migrationBuilder.DropTable(
                name: "Employee");
        }
    }
}
