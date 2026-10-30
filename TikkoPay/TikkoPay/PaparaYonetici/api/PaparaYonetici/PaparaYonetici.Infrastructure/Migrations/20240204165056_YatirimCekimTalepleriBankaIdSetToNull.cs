using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaYonetici.Infrastructure.Migrations
{
    public partial class YatirimCekimTalepleriBankaIdSetToNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YatirimTalepleri_Bankalar_BankaId",
                table: "YatirimTalepleri");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankaId",
                table: "YatirimTalepleri",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimTalepleri_Bankalar_BankaId",
                table: "YatirimTalepleri",
                column: "BankaId",
                principalTable: "Bankalar",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YatirimTalepleri_Bankalar_BankaId",
                table: "YatirimTalepleri");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankaId",
                table: "YatirimTalepleri",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimTalepleri_Bankalar_BankaId",
                table: "YatirimTalepleri",
                column: "BankaId",
                principalTable: "Bankalar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
