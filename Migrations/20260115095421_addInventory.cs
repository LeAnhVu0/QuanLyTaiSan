using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTaiSan.Migrations
{
    /// <inheritdoc />
    public partial class addInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InventoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookQuantity = table.Column<int>(type: "int", nullable: false),
                    ActualQuantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserIdBy = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventory_AspNetUsers_UserIdBy",
                        column: x => x.UserIdBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_DepartmentId",
                table: "Inventory",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_UserIdBy",
                table: "Inventory",
                column: "UserIdBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory");
        }
    }
}
