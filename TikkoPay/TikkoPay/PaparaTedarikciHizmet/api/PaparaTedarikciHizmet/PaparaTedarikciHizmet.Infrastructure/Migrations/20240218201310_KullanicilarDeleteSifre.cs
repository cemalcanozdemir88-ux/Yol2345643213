using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaTedarikciHizmet.Infrastructure.Migrations
{
    public partial class KullanicilarDeleteSifre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sifre",
                table: "Kullanicilar");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sifre",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
