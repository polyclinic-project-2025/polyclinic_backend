using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantityA",
                table: "Medication",
                newName: "QuantityWarehouse");

            migrationBuilder.AddColumn<int>(
                name: "MaxQuantityNurse",
                table: "Medication",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxQuantityWarehouse",
                table: "Medication",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinQuantityNurse",
                table: "Medication",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinQuantityWarehouse",
                table: "Medication",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxQuantityNurse",
                table: "Medication");

            migrationBuilder.DropColumn(
                name: "MaxQuantityWarehouse",
                table: "Medication");

            migrationBuilder.DropColumn(
                name: "MinQuantityNurse",
                table: "Medication");

            migrationBuilder.DropColumn(
                name: "MinQuantityWarehouse",
                table: "Medication");

            migrationBuilder.RenameColumn(
                name: "QuantityWarehouse",
                table: "Medication",
                newName: "QuantityA");
        }
    }
}
