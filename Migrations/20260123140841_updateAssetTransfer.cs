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
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransfer_Department_DepartmentId",
                table: "AssetTransfer");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "AssetTransfer",
                newName: "ToDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetTransfer_DepartmentId",
                table: "AssetTransfer",
                newName: "IX_AssetTransfer_ToDepartmentId");

            migrationBuilder.AddColumn<int>(
                name: "FromDepartmentId",
                table: "AssetTransfer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransfer_FromDepartmentId",
                table: "AssetTransfer",
                column: "FromDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransfer_Department_FromDepartmentId",
                table: "AssetTransfer",
                column: "FromDepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransfer_Department_ToDepartmentId",
                table: "AssetTransfer",
                column: "ToDepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransfer_Department_FromDepartmentId",
                table: "AssetTransfer");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransfer_Department_ToDepartmentId",
                table: "AssetTransfer");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransfer_FromDepartmentId",
                table: "AssetTransfer");

            migrationBuilder.DropColumn(
                name: "FromDepartmentId",
                table: "AssetTransfer");

            migrationBuilder.RenameColumn(
                name: "ToDepartmentId",
                table: "AssetTransfer",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetTransfer_ToDepartmentId",
                table: "AssetTransfer",
                newName: "IX_AssetTransfer_DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransfer_Department_DepartmentId",
                table: "AssetTransfer",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
