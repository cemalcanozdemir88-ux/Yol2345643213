using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikkoPayYonetici.Infrastructure.Migrations
{
    public partial class HavaleBankalariHesapSahibi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HesapSahibi",
                table: "HavaleBankalari",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HesapSahibi",
                table: "HavaleBankalari");
        }
    }
}
