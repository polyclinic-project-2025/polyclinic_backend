using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationPropertyEmergencyRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmergencyRoomCare_DoctorId_GuardDate",
                table: "EmergencyRoomCare",
                columns: new[] { "DoctorId", "GuardDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRoomCare_EmergencyRoom_DoctorId_GuardDate",
                table: "EmergencyRoomCare",
                columns: new[] { "DoctorId", "GuardDate" },
                principalTable: "EmergencyRoom",
                principalColumns: new[] { "DoctorId", "GuardDate" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRoomCare_EmergencyRoom_DoctorId_GuardDate",
                table: "EmergencyRoomCare");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyRoomCare_DoctorId_GuardDate",
                table: "EmergencyRoomCare");
        }
    }
}
