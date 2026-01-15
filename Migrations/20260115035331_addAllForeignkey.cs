using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTaiSan.Migrations
{
    /// <inheritdoc />
    public partial class addAllForeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "OriginalValue",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AssetHistory");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Report",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Assets",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserId",
                table: "AssetHistory",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "AssetHistory",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Report_UserId",
                table: "Report",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_DepartmentId",
                table: "Assets",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_UserId",
                table: "Assets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistory_AssignedToUserId",
                table: "AssetHistory",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetHistory_CreatedByUserId",
                table: "AssetHistory",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetHistory_AspNetUsers_AssignedToUserId",
                table: "AssetHistory",
                column: "AssignedToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetHistory_AspNetUsers_CreatedByUserId",
                table: "AssetHistory",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AspNetUsers_UserId",
                table: "Assets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Department_DepartmentId",
                table: "Assets",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AspNetUsers_UserId",
                table: "Report",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetHistory_AspNetUsers_AssignedToUserId",
                table: "AssetHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetHistory_AspNetUsers_CreatedByUserId",
                table: "AssetHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AspNetUsers_UserId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Department_DepartmentId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AspNetUsers_UserId",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_UserId",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Assets_DepartmentId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_UserId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_AssetHistory_AssignedToUserId",
                table: "AssetHistory");

            migrationBuilder.DropIndex(
                name: "IX_AssetHistory_CreatedByUserId",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "AssetHistory");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AssetHistory");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Report",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Assets",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "AssetHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalValue",
                table: "AssetHistory",
                type: "decimal(19,0)",
                precision: 19,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "AssetHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "AssetHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AssetHistory",
                type: "int",
                nullable: true);
        }
    }
}
