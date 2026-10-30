using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikkoPayYonetici.Infrastructure.Migrations
{
    public partial class DurumAddedToKasaHaraketleri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Durum",
                table: "KasaHaraketleri",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "ResimUrl",
                table: "KasaHaraketleri",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Durum",
                table: "KasaHaraketleri");

            migrationBuilder.DropColumn(
                name: "ResimUrl",
                table: "KasaHaraketleri");
        }
    }
}
