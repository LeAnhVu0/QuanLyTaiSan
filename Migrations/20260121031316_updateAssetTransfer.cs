using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTaiSan.Migrations
{
    /// <inheritdoc />
    public partial class updateAssetTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "AssetTransfer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "AssetTransfer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransfer_DepartmentId",
                table: "AssetTransfer",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransfer_Department_DepartmentId",
                table: "AssetTransfer",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransfer_Department_DepartmentId",
                table: "AssetTransfer");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransfer_DepartmentId",
                table: "AssetTransfer");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AssetTransfer");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "AssetTransfer");
        }
    }
}
