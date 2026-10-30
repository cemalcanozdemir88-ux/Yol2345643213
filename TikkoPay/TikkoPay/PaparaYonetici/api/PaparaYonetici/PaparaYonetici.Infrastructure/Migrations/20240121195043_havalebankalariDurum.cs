using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaYonetici.Infrastructure.Migrations
{
    public partial class havalebankalariDurum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Durum",
                table: "HavaleBankalari",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Durum",
                table: "HavaleBankalari");
        }
    }
}
