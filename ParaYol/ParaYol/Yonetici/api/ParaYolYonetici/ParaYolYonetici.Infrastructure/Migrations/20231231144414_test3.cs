using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParaYolYonetici.Infrastructure.Migrations
{
    public partial class test3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_WebSiteleri_WebSiteId",
                table: "Kullanicilar");

            //migrationBuilder.DropIndex(
            //    name: "IX_Kullanicilar_WebSiteId",
            //    table: "Kullanicilar");

            //migrationBuilder.DropColumn(
            //    name: "WebSiteId",
            //    table: "Kullanicilar");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_WebSiteId",
                table: "Kullanicilar",
                column: "WebSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_WebSiteleri_WebSiteId",
                table: "Kullanicilar",
                column: "WebSiteId",
                principalTable: "WebSiteleri",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_WebSiteleri_WebSiteId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_WebSiteId",
                table: "Kullanicilar");

            migrationBuilder.AddColumn<Guid>(
                name: "WebSiteId",
                table: "Kullanicilar",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_WebSiteId",
                table: "Kullanicilar",
                column: "WebSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_WebSiteleri_WebSiteId",
                table: "Kullanicilar",
                column: "WebSiteId",
                principalTable: "WebSiteleri",
                principalColumn: "Id");
        }
    }
}
