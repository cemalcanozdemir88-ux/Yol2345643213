using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParaYolYonetici.Infrastructure.Migrations
{
    public partial class DepositTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepositTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ClientSecret = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Hash = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositTransactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepositTransactions");
        }
    }
}
