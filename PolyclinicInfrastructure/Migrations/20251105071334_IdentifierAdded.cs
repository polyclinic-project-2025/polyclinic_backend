using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IdentifierAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patient_Contact",
                table: "Patient");

            migrationBuilder.AddColumn<int>(
                name: "Identification",
                table: "Patient",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Identification",
                table: "Employee",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Identification",
                table: "Patient",
                column: "Identification",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Identification",
                table: "Employee",
                column: "Identification",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patient_Identification",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Employee_Identification",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Identification",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Identification",
                table: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Contact",
                table: "Patient",
                column: "Contact",
                unique: true);
        }
    }
}
