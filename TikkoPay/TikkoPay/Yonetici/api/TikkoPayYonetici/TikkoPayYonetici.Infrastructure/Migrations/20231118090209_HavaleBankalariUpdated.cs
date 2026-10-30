using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikkoPayYonetici.Infrastructure.Migrations
{
    public partial class HavaleBankalariUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HesapAdedi",
                table: "HavaleBankalari");

            migrationBuilder.AddColumn<string>(
                name: "HesapNo",
                table: "HavaleBankalari",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HesapNo",
                table: "HavaleBankalari");

            migrationBuilder.AddColumn<int>(
                name: "HesapAdedi",
                table: "HavaleBankalari",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
