using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaYonetici.Infrastructure.Migrations
{
    public partial class AmountAddedToDepositTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Amount",
                table: "DepositTransactions",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "DepositTransactions");
        }
    }
}
