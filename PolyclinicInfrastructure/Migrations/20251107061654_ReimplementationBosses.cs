using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyclinicInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReimplementationBosses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Derivation_Boss_BossId",
                table: "Consultation Derivation");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Referral_Boss_BossId",
                table: "Consultation Referral");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Boss_BossId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Nursing_Boss_BossId",
                table: "Nursing");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_Boss_BossId",
                table: "Warehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseRequest_Boss_BossId",
                table: "WarehouseRequest");

            migrationBuilder.DropTable(
                name: "Boss");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_BossId",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_Nursing_BossId",
                table: "Nursing");

            migrationBuilder.DropIndex(
                name: "IX_Department_BossId",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "BossId",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "BossId",
                table: "Nursing");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "BossId",
                table: "Department");

            migrationBuilder.RenameColumn(
                name: "BossId",
                table: "Consultation Referral",
                newName: "ApprovedByHeadId");

            migrationBuilder.RenameIndex(
                name: "IX_Consultation Referral_BossId",
                table: "Consultation Referral",
                newName: "IX_Consultation Referral_ApprovedByHeadId");

            migrationBuilder.RenameColumn(
                name: "BossId",
                table: "Consultation Derivation",
                newName: "ApprovedByHeadId");

            migrationBuilder.RenameIndex(
                name: "IX_Consultation Derivation_BossId",
                table: "Consultation Derivation",
                newName: "IX_Consultation Derivation_ApprovedByHeadId");

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Warehouse",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HeadId",
                table: "Nursing",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HeadId",
                table: "Department",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DepartmentHead",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagedDepartmentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentHead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentHead_Employee_Id",
                        column: x => x.Id,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NursingHead",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagedNursingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NursingHead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NursingHead_Employee_Id",
                        column: x => x.Id,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseManager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagedWarehouseId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseManager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseManager_Employee_Id",
                        column: x => x.Id,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_ManagerId",
                table: "Warehouse",
                column: "ManagerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nursing_HeadId",
                table: "Nursing",
                column: "HeadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_HeadId",
                table: "Department",
                column: "HeadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Derivation_DepartmentHead_ApprovedByHeadId",
                table: "Consultation Derivation",
                column: "ApprovedByHeadId",
                principalTable: "DepartmentHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Referral_DepartmentHead_ApprovedByHeadId",
                table: "Consultation Referral",
                column: "ApprovedByHeadId",
                principalTable: "DepartmentHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_DepartmentHead_HeadId",
                table: "Department",
                column: "HeadId",
                principalTable: "DepartmentHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nursing_NursingHead_HeadId",
                table: "Nursing",
                column: "HeadId",
                principalTable: "NursingHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_WarehouseManager_ManagerId",
                table: "Warehouse",
                column: "ManagerId",
                principalTable: "WarehouseManager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseRequest_DepartmentHead_BossId",
                table: "WarehouseRequest",
                column: "BossId",
                principalTable: "DepartmentHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Derivation_DepartmentHead_ApprovedByHeadId",
                table: "Consultation Derivation");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultation Referral_DepartmentHead_ApprovedByHeadId",
                table: "Consultation Referral");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_DepartmentHead_HeadId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_Nursing_NursingHead_HeadId",
                table: "Nursing");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_WarehouseManager_ManagerId",
                table: "Warehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseRequest_DepartmentHead_BossId",
                table: "WarehouseRequest");

            migrationBuilder.DropTable(
                name: "DepartmentHead");

            migrationBuilder.DropTable(
                name: "NursingHead");

            migrationBuilder.DropTable(
                name: "WarehouseManager");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_ManagerId",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_Nursing_HeadId",
                table: "Nursing");

            migrationBuilder.DropIndex(
                name: "IX_Department_HeadId",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "HeadId",
                table: "Nursing");

            migrationBuilder.DropColumn(
                name: "HeadId",
                table: "Department");

            migrationBuilder.RenameColumn(
                name: "ApprovedByHeadId",
                table: "Consultation Referral",
                newName: "BossId");

            migrationBuilder.RenameIndex(
                name: "IX_Consultation Referral_ApprovedByHeadId",
                table: "Consultation Referral",
                newName: "IX_Consultation Referral_BossId");

            migrationBuilder.RenameColumn(
                name: "ApprovedByHeadId",
                table: "Consultation Derivation",
                newName: "BossId");

            migrationBuilder.RenameIndex(
                name: "IX_Consultation Derivation_ApprovedByHeadId",
                table: "Consultation Derivation",
                newName: "IX_Consultation Derivation_BossId");

            migrationBuilder.AddColumn<Guid>(
                name: "BossId",
                table: "Warehouse",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BossId",
                table: "Nursing",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Employee",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "BossId",
                table: "Department",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_BossId",
                table: "Warehouse",
                column: "BossId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nursing_BossId",
                table: "Nursing",
                column: "BossId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_BossId",
                table: "Department",
                column: "BossId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Derivation_Boss_BossId",
                table: "Consultation Derivation",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultation Referral_Boss_BossId",
                table: "Consultation Referral",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Boss_BossId",
                table: "Department",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nursing_Boss_BossId",
                table: "Nursing",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_Boss_BossId",
                table: "Warehouse",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseRequest_Boss_BossId",
                table: "WarehouseRequest",
                column: "BossId",
                principalTable: "Boss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
