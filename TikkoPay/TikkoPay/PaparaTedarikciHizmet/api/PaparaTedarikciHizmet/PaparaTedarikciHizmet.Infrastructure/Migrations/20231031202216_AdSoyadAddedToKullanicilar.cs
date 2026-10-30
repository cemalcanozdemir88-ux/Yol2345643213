using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaTedarikciHizmet.Infrastructure.Migrations
{
    public partial class AdSoyadAddedToKullanicilar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdSoyad",
                table: "Kullanicilar",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdSoyad",
                table: "Kullanicilar");
        }
    }
}
