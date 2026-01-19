using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTaiSan.Migrations
{
    /// <inheritdoc />
    public partial class addCheckDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Assets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Assets");
        }
    }
}
