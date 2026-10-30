using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaYonetici.Infrastructure.Migrations
{
    public partial class durumDefaultvalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Durum",
                table: "HavaleBankalari",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Durum",
                table: "HavaleBankalari",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
