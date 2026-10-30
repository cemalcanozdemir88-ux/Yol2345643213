using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TedarikciHizmet.Infrastructure.Migrations
{
    public partial class AdminMiAddedToTedarikciler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdminMi",
                table: "Tedarikciler",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminMi",
                table: "Tedarikciler");
        }
    }
}
