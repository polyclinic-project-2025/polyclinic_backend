using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WarehouseManager");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Nurse");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MedicalStaff");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DepartmentHead");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Employee",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Employee");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WarehouseManager",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Nurse",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MedicalStaff",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DepartmentHead",
                type: "text",
                nullable: true);
        }
    }
}
