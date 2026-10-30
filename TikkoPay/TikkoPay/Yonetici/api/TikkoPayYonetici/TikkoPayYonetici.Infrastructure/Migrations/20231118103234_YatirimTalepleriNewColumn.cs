using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikkoPayYonetici.Infrastructure.Migrations
{
    public partial class YatirimTalepleriNewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdNumber",
                table: "YatirimTalepleri",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1000, 1");

            migrationBuilder.AddColumn<string>(
                name: "TalepNo",
                table: "YatirimTalepleri",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdNumber",
                table: "YatirimTalepleri");

            migrationBuilder.DropColumn(
                name: "TalepNo",
                table: "YatirimTalepleri");
        }
    }
}
