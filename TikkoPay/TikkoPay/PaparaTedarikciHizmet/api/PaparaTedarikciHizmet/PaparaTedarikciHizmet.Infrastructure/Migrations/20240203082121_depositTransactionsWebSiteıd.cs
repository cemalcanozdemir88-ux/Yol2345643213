using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaTedarikciHizmet.Infrastructure.Migrations
{
    public partial class depositTransactionsWebSiteıd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WebSiteId",
                table: "DepositTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebSiteId",
                table: "DepositTransactions");
        }
    }
}
