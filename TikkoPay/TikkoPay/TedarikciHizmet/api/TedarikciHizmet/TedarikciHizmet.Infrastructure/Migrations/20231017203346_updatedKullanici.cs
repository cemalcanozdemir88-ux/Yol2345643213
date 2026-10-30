using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TedarikciHizmet.Infrastructure.Migrations
{
    public partial class updatedKullanici : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TedarikciAnaKullanici",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "Unvan",
                table: "Kullanicilar");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TedarikciAnaKullanici",
                table: "Kullanicilar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Unvan",
                table: "Kullanicilar",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
