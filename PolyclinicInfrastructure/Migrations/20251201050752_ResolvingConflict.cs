using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResolvingConflict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "WarehouseRequest",
                schema: "public",
                newName: "WarehouseRequest");

            migrationBuilder.RenameTable(
                name: "WarehouseManager",
                schema: "public",
                newName: "WarehouseManager");

            migrationBuilder.RenameTable(
                name: "Warehouse",
                schema: "public",
                newName: "Warehouse");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "public",
                newName: "UserTokens");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "public",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "public",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "public",
                newName: "UserLogins");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "public",
                newName: "UserClaims");

            migrationBuilder.RenameTable(
                name: "StockDepartment",
                schema: "public",
                newName: "StockDepartment");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "public",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "public",
                newName: "RoleClaims");

            migrationBuilder.RenameTable(
                name: "Referral",
                schema: "public",
                newName: "Referral");

            migrationBuilder.RenameTable(
                name: "Patient",
                schema: "public",
                newName: "Patient");

            migrationBuilder.RenameTable(
                name: "Nursing",
                schema: "public",
                newName: "Nursing");

            migrationBuilder.RenameTable(
                name: "Nurse",
                schema: "public",
                newName: "Nurse");

            migrationBuilder.RenameTable(
                name: "MedicationRequest",
                schema: "public",
                newName: "MedicationRequest");

            migrationBuilder.RenameTable(
                name: "MedicationReferral",
                schema: "public",
                newName: "MedicationReferral");

            migrationBuilder.RenameTable(
                name: "MedicationEmergency",
                schema: "public",
                newName: "MedicationEmergency");

            migrationBuilder.RenameTable(
                name: "MedicationDerivation",
                schema: "public",
                newName: "MedicationDerivation");

            migrationBuilder.RenameTable(
                name: "Medication",
                schema: "public",
                newName: "Medication");

            migrationBuilder.RenameTable(
                name: "MedicalStaff",
                schema: "public",
                newName: "MedicalStaff");

            migrationBuilder.RenameTable(
                name: "ExternalMedicalPost",
                schema: "public",
                newName: "ExternalMedicalPost");

            migrationBuilder.RenameTable(
                name: "Employee",
                schema: "public",
                newName: "Employee");

            migrationBuilder.RenameTable(
                name: "EmergencyRoomCare",
                schema: "public",
                newName: "EmergencyRoomCare");

            migrationBuilder.RenameTable(
                name: "EmergencyRoom",
                schema: "public",
                newName: "EmergencyRoom");

            migrationBuilder.RenameTable(
                name: "Doctor",
                schema: "public",
                newName: "Doctor");

            migrationBuilder.RenameTable(
                name: "Derivation",
                schema: "public",
                newName: "Derivation");

            migrationBuilder.RenameTable(
                name: "DepartmentHead",
                schema: "public",
                newName: "DepartmentHead");

            migrationBuilder.RenameTable(
                name: "Department",
                schema: "public",
                newName: "Department");

            migrationBuilder.RenameTable(
                name: "ConsultationReferral",
                schema: "public",
                newName: "ConsultationReferral");

            migrationBuilder.RenameTable(
                name: "ConsultationDerivation",
                schema: "public",
                newName: "ConsultationDerivation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "WarehouseRequest",
                newName: "WarehouseRequest",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "WarehouseManager",
                newName: "WarehouseManager",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Warehouse",
                newName: "Warehouse",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                newName: "UserTokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                newName: "UserLogins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                newName: "UserClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "StockDepartment",
                newName: "StockDepartment",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                newName: "RoleClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Referral",
                newName: "Referral",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Patient",
                newName: "Patient",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Nursing",
                newName: "Nursing",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Nurse",
                newName: "Nurse",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "MedicationRequest",
                newName: "MedicationRequest",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "MedicationReferral",
                newName: "MedicationReferral",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "MedicationEmergency",
                newName: "MedicationEmergency",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "MedicationDerivation",
                newName: "MedicationDerivation",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Medication",
                newName: "Medication",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "MedicalStaff",
                newName: "MedicalStaff",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ExternalMedicalPost",
                newName: "ExternalMedicalPost",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "Employee",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "EmergencyRoomCare",
                newName: "EmergencyRoomCare",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "EmergencyRoom",
                newName: "EmergencyRoom",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Doctor",
                newName: "Doctor",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Derivation",
                newName: "Derivation",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "DepartmentHead",
                newName: "DepartmentHead",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "Department",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ConsultationReferral",
                newName: "ConsultationReferral",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ConsultationDerivation",
                newName: "ConsultationDerivation",
                newSchema: "public");
        }
    }
}
