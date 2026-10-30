using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaYonetici.Infrastructure.Migrations
{
    public partial class WebSiteleriOdemeKomisyonOrani : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "OdemeKomisyonOrani",
                table: "WebSiteleri",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OdemeKomisyonOrani",
                table: "WebSiteleri");
        }
    }
}
