using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParaYolYonetici.Infrastructure.Migrations
{
    public partial class WebSiteleriAndKullanicilariCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebSiteleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebSiteUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    YaririmKomisyonOrani = table.Column<double>(type: "float", nullable: false),
                    CekimKomisyonOrani = table.Column<double>(type: "float", nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSiteleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebSiteKullanicilari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Durum = table.Column<bool>(type: "bit", nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSiteKullanicilari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebSiteKullanicilari_WebSiteleri_WebSiteId",
                        column: x => x.WebSiteId,
                        principalTable: "WebSiteleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebSiteKullanicilari_WebSiteId",
                table: "WebSiteKullanicilari",
                column: "WebSiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebSiteKullanicilari");

            migrationBuilder.DropTable(
                name: "WebSiteleri");
        }
    }
}
