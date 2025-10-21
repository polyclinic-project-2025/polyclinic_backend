using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBossNursing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nursing_Employees_BossId",
                table: "Nursing");

            migrationBuilder.DropIndex(
                name: "IX_Nursing_BossId",
                table: "Nursing");

            migrationBuilder.DropColumn(
                name: "BossId",
                table: "Nursing");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BossId",
                table: "Nursing",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nursing_BossId",
                table: "Nursing",
                column: "BossId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nursing_Employees_BossId",
                table: "Nursing",
                column: "BossId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
