using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TedarikciHizmet.Infrastructure.Migrations
{
    public partial class IslemTipiNameChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "İslemTipi",
                table: "YatirimTalepleri",
                newName: "IslemTipi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IslemTipi",
                table: "YatirimTalepleri",
                newName: "İslemTipi");
        }
    }
}
