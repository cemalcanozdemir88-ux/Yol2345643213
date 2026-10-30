using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikkoPayYonetici.Infrastructure.Migrations
{
    public partial class WebSiteKomisyonColumnCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Komisyon",
                table: "YatirimTalepleri",
                newName: "TedarikciKomisyon"); 

            migrationBuilder.AddColumn<decimal>(
                name: "WebSiteKomisyon",
                table: "YatirimTalepleri",
                type: "decimal(13,4)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebSiteKomisyon",
                table: "YatirimTalepleri");

            migrationBuilder.RenameColumn(
                name: "TedarikciKomisyon",
                table: "YatirimTalepleri",
                newName: "Komisyon");
        }
    }
}
