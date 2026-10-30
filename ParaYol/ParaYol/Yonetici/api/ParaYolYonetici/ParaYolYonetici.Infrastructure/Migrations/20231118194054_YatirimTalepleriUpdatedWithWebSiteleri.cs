using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParaYolYonetici.Infrastructure.Migrations
{
    public partial class YatirimTalepleriUpdatedWithWebSiteleri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebSiteUrl",
                table: "YatirimTalepleri");

            migrationBuilder.AddColumn<Guid>(
                name: "WebSiteId",
                table: "YatirimTalepleri",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_YatirimTalepleri_WebSiteId",
                table: "YatirimTalepleri",
                column: "WebSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimTalepleri_WebSiteleri_WebSiteId",
                table: "YatirimTalepleri",
                column: "WebSiteId",
                principalTable: "WebSiteleri",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YatirimTalepleri_WebSiteleri_WebSiteId",
                table: "YatirimTalepleri");

            migrationBuilder.DropIndex(
                name: "IX_YatirimTalepleri_WebSiteId",
                table: "YatirimTalepleri");

            migrationBuilder.DropColumn(
                name: "WebSiteId",
                table: "YatirimTalepleri");

            migrationBuilder.AddColumn<string>(
                name: "WebSiteUrl",
                table: "YatirimTalepleri",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
