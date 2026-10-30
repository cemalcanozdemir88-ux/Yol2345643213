using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TedarikciHizmet.Infrastructure.Migrations
{
    public partial class TutarChangeToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Komisyon",
                table: "YatirimTalepleri",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Komisyon",
                table: "YatirimTalepleri",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
