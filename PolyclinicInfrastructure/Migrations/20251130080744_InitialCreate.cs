using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Department",
                schema: "public",
                columns: table => new
                {
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "ExternalMedicalPost",
                schema: "public",
                columns: table => new
                {
                    ExternalMedicalPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalMedicalPost", x => x.ExternalMedicalPostId);
                });

            migrationBuilder.CreateTable(
                name: "Medication",
                schema: "public",
                columns: table => new
                {
                    MedicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Format = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CommercialName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CommercialCompany = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ScientificName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    QuantityA = table.Column<int>(type: "integer", nullable: false),
                    QuantityNurse = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medication", x => x.MedicationId);
                });

            migrationBuilder.CreateTable(
                name: "Nursing",
                schema: "public",
                columns: table => new
                {
                    NursingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nursing", x => x.NursingId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                schema: "public",
                columns: table => new
                {
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.WarehouseId);
                });

            migrationBuilder.CreateTable(
                name: "StockDepartment",
                schema: "public",
                columns: table => new
                {
                    StockDepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDepartment", x => x.StockDepartmentId);
                    table.ForeignKey(
                        name: "FK_StockDepartment_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "public",
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockDepartment_Medication_MedicationId",
                        column: x => x.MedicationId,
                        principalSchema: "public",
                        principalTable: "Medication",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                schema: "public",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Identification = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmploymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employee_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                schema: "public",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Identification = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Contact = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patient_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "public",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "public",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "public",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentHead",
                schema: "public",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentHead", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_DepartmentHead_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "public",
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentHead_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "public",
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalStaff",
                schema: "public",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalStaff", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_MedicalStaff_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "public",
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalStaff_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "public",
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nurse",
                schema: "public",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NursingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nurse", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Nurse_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "public",
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Nurse_Nursing_NursingId",
                        column: x => x.NursingId,
                        principalSchema: "public",
                        principalTable: "Nursing",
                        principalColumn: "NursingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseManager",
                schema: "public",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseManager", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_WarehouseManager_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "public",
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseManager_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "public",
                        principalTable: "Warehouse",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Derivation",
                schema: "public",
                columns: table => new
                {
                    DerivationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentFromId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeDer = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Derivation", x => x.DerivationId);
                    table.ForeignKey(
                        name: "FK_Derivation_Department_DepartmentFromId",
                        column: x => x.DepartmentFromId,
                        principalSchema: "public",
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Derivation_Department_DepartmentToId",
                        column: x => x.DepartmentToId,
                        principalSchema: "public",
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Derivation_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Referral",
                schema: "public",
                columns: table => new
                {
                    ReferralId = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeRem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalMedicalPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentToId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referral", x => x.ReferralId);
                    table.ForeignKey(
                        name: "FK_Referral_Department_DepartmentToId",
                        column: x => x.DepartmentToId,
                        principalSchema: "public",
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referral_ExternalMedicalPost_ExternalMedicalPostId",
                        column: x => x.ExternalMedicalPostId,
                        principalSchema: "public",
                        principalTable: "ExternalMedicalPost",
                        principalColumn: "ExternalMedicalPostId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referral_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Doctor",
                schema: "public",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Doctor_MedicalStaff_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "public",
                        principalTable: "MedicalStaff",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseRequest",
                schema: "public",
                columns: table => new
                {
                    WarehouseRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentHeadEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseRequest", x => x.WarehouseRequestId);
                    table.ForeignKey(
                        name: "FK_WarehouseRequest_DepartmentHead_DepartmentHeadEmployeeId",
                        column: x => x.DepartmentHeadEmployeeId,
                        principalSchema: "public",
                        principalTable: "DepartmentHead",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_WarehouseRequest_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "public",
                        principalTable: "Department",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseRequest_WarehouseManager_WarehouseManagerId",
                        column: x => x.WarehouseManagerId,
                        principalSchema: "public",
                        principalTable: "WarehouseManager",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseRequest_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "public",
                        principalTable: "Warehouse",
                        principalColumn: "WarehouseId");
                });

            migrationBuilder.CreateTable(
                name: "ConsultationDerivation",
                schema: "public",
                columns: table => new
                {
                    ConsultationDerivationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DepartmentHeadId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DerivationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeCDer = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentHeadEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationDerivation", x => x.ConsultationDerivationId);
                    table.ForeignKey(
                        name: "FK_ConsultationDerivation_DepartmentHead_DepartmentHeadEmploye~",
                        column: x => x.DepartmentHeadEmployeeId,
                        principalSchema: "public",
                        principalTable: "DepartmentHead",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ConsultationDerivation_DepartmentHead_DepartmentHeadId",
                        column: x => x.DepartmentHeadId,
                        principalSchema: "public",
                        principalTable: "DepartmentHead",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationDerivation_Derivation_DerivationId",
                        column: x => x.DerivationId,
                        principalSchema: "public",
                        principalTable: "Derivation",
                        principalColumn: "DerivationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationDerivation_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "public",
                        principalTable: "Doctor",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationDerivation_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                });

            migrationBuilder.CreateTable(
                name: "ConsultationReferral",
                schema: "public",
                columns: table => new
                {
                    ConsultationReferralId = table.Column<Guid>(type: "uuid", nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DepartmentHeadId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferralId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTimeCRem = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentHeadEmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationReferral", x => x.ConsultationReferralId);
                    table.ForeignKey(
                        name: "FK_ConsultationReferral_DepartmentHead_DepartmentHeadEmployeeId",
                        column: x => x.DepartmentHeadEmployeeId,
                        principalSchema: "public",
                        principalTable: "DepartmentHead",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ConsultationReferral_DepartmentHead_DepartmentHeadId",
                        column: x => x.DepartmentHeadId,
                        principalSchema: "public",
                        principalTable: "DepartmentHead",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationReferral_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "public",
                        principalTable: "Doctor",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationReferral_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "Patient",
                        principalColumn: "PatientId");
                    table.ForeignKey(
                        name: "FK_ConsultationReferral_Referral_ReferralId",
                        column: x => x.ReferralId,
                        principalSchema: "public",
                        principalTable: "Referral",
                        principalColumn: "ReferralId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyRoom",
                schema: "public",
                columns: table => new
                {
                    EmergencyRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuardDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyRoom", x => x.EmergencyRoomId);
                    table.ForeignKey(
                        name: "FK_EmergencyRoom_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "public",
                        principalTable: "Doctor",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicationRequest",
                schema: "public",
                columns: table => new
                {
                    MedicationRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    WarehouseRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationRequest", x => x.MedicationRequestId);
                    table.ForeignKey(
                        name: "FK_MedicationRequest_Medication_MedicationId",
                        column: x => x.MedicationId,
                        principalSchema: "public",
                        principalTable: "Medication",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicationRequest_WarehouseRequest_WarehouseRequestId",
                        column: x => x.WarehouseRequestId,
                        principalSchema: "public",
                        principalTable: "WarehouseRequest",
                        principalColumn: "WarehouseRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicationDerivation",
                schema: "public",
                columns: table => new
                {
                    MedicationDerivationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ConsultationDerivationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDerivation", x => x.MedicationDerivationId);
                    table.ForeignKey(
                        name: "FK_MedicationDerivation_ConsultationDerivation_ConsultationDer~",
                        column: x => x.ConsultationDerivationId,
                        principalSchema: "public",
                        principalTable: "ConsultationDerivation",
                        principalColumn: "ConsultationDerivationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicationDerivation_Medication_MedicationId",
                        column: x => x.MedicationId,
                        principalSchema: "public",
                        principalTable: "Medication",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicationReferral",
                schema: "public",
                columns: table => new
                {
                    MedicationReferralId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ConsultationReferralId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationReferral", x => x.MedicationReferralId);
                    table.ForeignKey(
                        name: "FK_MedicationReferral_ConsultationReferral_ConsultationReferra~",
                        column: x => x.ConsultationReferralId,
                        principalSchema: "public",
                        principalTable: "ConsultationReferral",
                        principalColumn: "ConsultationReferralId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicationReferral_Medication_MedicationId",
                        column: x => x.MedicationId,
                        principalSchema: "public",
                        principalTable: "Medication",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyRoomCare",
                schema: "public",
                columns: table => new
                {
                    EmergencyRoomCareId = table.Column<Guid>(type: "uuid", nullable: false),
                    Diagnosis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    EmergencyRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    CareDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyRoomCare", x => x.EmergencyRoomCareId);
                    table.ForeignKey(
                        name: "FK_EmergencyRoomCare_EmergencyRoom_EmergencyRoomId",
                        column: x => x.EmergencyRoomId,
                        principalSchema: "public",
                        principalTable: "EmergencyRoom",
                        principalColumn: "EmergencyRoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmergencyRoomCare_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "public",
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicationEmergency",
                schema: "public",
                columns: table => new
                {
                    MedicationEmergencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    EmergencyRoomCareId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationEmergency", x => x.MedicationEmergencyId);
                    table.ForeignKey(
                        name: "FK_MedicationEmergency_EmergencyRoomCare_EmergencyRoomCareId",
                        column: x => x.EmergencyRoomCareId,
                        principalSchema: "public",
                        principalTable: "EmergencyRoomCare",
                        principalColumn: "EmergencyRoomCareId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicationEmergency_Medication_MedicationId",
                        column: x => x.MedicationId,
                        principalSchema: "public",
                        principalTable: "Medication",
                        principalColumn: "MedicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationDerivation_DepartmentHeadEmployeeId",
                schema: "public",
                table: "ConsultationDerivation",
                column: "DepartmentHeadEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationDerivation_DepartmentHeadId",
                schema: "public",
                table: "ConsultationDerivation",
                column: "DepartmentHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationDerivation_DerivationId",
                schema: "public",
                table: "ConsultationDerivation",
                column: "DerivationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationDerivation_DoctorId_DerivationId_DateTimeCDer",
                schema: "public",
                table: "ConsultationDerivation",
                columns: new[] { "DoctorId", "DerivationId", "DateTimeCDer" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationDerivation_PatientId",
                schema: "public",
                table: "ConsultationDerivation",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationReferral_DepartmentHeadEmployeeId",
                schema: "public",
                table: "ConsultationReferral",
                column: "DepartmentHeadEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationReferral_DepartmentHeadId",
                schema: "public",
                table: "ConsultationReferral",
                column: "DepartmentHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationReferral_DoctorId_ReferralId_DateTimeCRem",
                schema: "public",
                table: "ConsultationReferral",
                columns: new[] { "DoctorId", "ReferralId", "DateTimeCRem" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationReferral_PatientId",
                schema: "public",
                table: "ConsultationReferral",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationReferral_ReferralId",
                schema: "public",
                table: "ConsultationReferral",
                column: "ReferralId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentHead_DepartmentId",
                schema: "public",
                table: "DepartmentHead",
                column: "DepartmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Derivation_DepartmentFromId_DateTimeDer_PatientId",
                schema: "public",
                table: "Derivation",
                columns: new[] { "DepartmentFromId", "DateTimeDer", "PatientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Derivation_DepartmentToId",
                schema: "public",
                table: "Derivation",
                column: "DepartmentToId");

            migrationBuilder.CreateIndex(
                name: "IX_Derivation_PatientId",
                schema: "public",
                table: "Derivation",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoom_DoctorId_GuardDate",
                schema: "public",
                table: "EmergencyRoom",
                columns: new[] { "DoctorId", "GuardDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoomCare_CareDate_PatientId",
                schema: "public",
                table: "EmergencyRoomCare",
                columns: new[] { "CareDate", "PatientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoomCare_EmergencyRoomId",
                schema: "public",
                table: "EmergencyRoomCare",
                column: "EmergencyRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoomCare_PatientId",
                schema: "public",
                table: "EmergencyRoomCare",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Identification",
                schema: "public",
                table: "Employee",
                column: "Identification",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_UserId",
                schema: "public",
                table: "Employee",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalStaff_DepartmentId",
                schema: "public",
                table: "MedicalStaff",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDerivation_ConsultationDerivationId_MedicationId",
                schema: "public",
                table: "MedicationDerivation",
                columns: new[] { "ConsultationDerivationId", "MedicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDerivation_MedicationId",
                schema: "public",
                table: "MedicationDerivation",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationEmergency_EmergencyRoomCareId_MedicationId",
                schema: "public",
                table: "MedicationEmergency",
                columns: new[] { "EmergencyRoomCareId", "MedicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationEmergency_MedicationId",
                schema: "public",
                table: "MedicationEmergency",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReferral_ConsultationReferralId_MedicationId",
                schema: "public",
                table: "MedicationReferral",
                columns: new[] { "ConsultationReferralId", "MedicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationReferral_MedicationId",
                schema: "public",
                table: "MedicationReferral",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRequest_MedicationId",
                schema: "public",
                table: "MedicationRequest",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationRequest_WarehouseRequestId_MedicationId",
                schema: "public",
                table: "MedicationRequest",
                columns: new[] { "WarehouseRequestId", "MedicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nurse_NursingId",
                schema: "public",
                table: "Nurse",
                column: "NursingId");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Identification",
                schema: "public",
                table: "Patient",
                column: "Identification",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patient_UserId",
                schema: "public",
                table: "Patient",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Referral_DepartmentToId",
                schema: "public",
                table: "Referral",
                column: "DepartmentToId");

            migrationBuilder.CreateIndex(
                name: "IX_Referral_ExternalMedicalPostId",
                schema: "public",
                table: "Referral",
                column: "ExternalMedicalPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Referral_PatientId_DateTimeRem_ExternalMedicalPostId",
                schema: "public",
                table: "Referral",
                columns: new[] { "PatientId", "DateTimeRem", "ExternalMedicalPostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "public",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "public",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockDepartment_DepartmentId_MedicationId",
                schema: "public",
                table: "StockDepartment",
                columns: new[] { "DepartmentId", "MedicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockDepartment_MedicationId",
                schema: "public",
                table: "StockDepartment",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "public",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "public",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "public",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "public",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "public",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseManager_WarehouseId",
                schema: "public",
                table: "WarehouseManager",
                column: "WarehouseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_DepartmentHeadEmployeeId",
                schema: "public",
                table: "WarehouseRequest",
                column: "DepartmentHeadEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_DepartmentId",
                schema: "public",
                table: "WarehouseRequest",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_RequestDate_DepartmentId",
                schema: "public",
                table: "WarehouseRequest",
                columns: new[] { "RequestDate", "DepartmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_WarehouseId",
                schema: "public",
                table: "WarehouseRequest",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseRequest_WarehouseManagerId",
                schema: "public",
                table: "WarehouseRequest",
                column: "WarehouseManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicationDerivation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MedicationEmergency",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MedicationReferral",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MedicationRequest",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Nurse",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StockDepartment",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ConsultationDerivation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EmergencyRoomCare",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ConsultationReferral",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WarehouseRequest",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Nursing",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Medication",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Derivation",
                schema: "public");

            migrationBuilder.DropTable(
                name: "EmergencyRoom",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Referral",
                schema: "public");

            migrationBuilder.DropTable(
                name: "DepartmentHead",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WarehouseManager",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Doctor",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ExternalMedicalPost",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Patient",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Warehouse",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MedicalStaff",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Department",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Employee",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "public");
        }
    }
}
