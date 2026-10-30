using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaYonetici.Infrastructure.Migrations
{
    public partial class WebSiteKasaHaraketleriCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebSiteKasaHaraketleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebSitesiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    UsdtMiktar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    UsdtAdres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UsdtTxtNo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ResimUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IslemTipi = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSiteKasaHaraketleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebSiteKasaHaraketleri_Kullanicilar_GuncelleyenKullaniciId",
                        column: x => x.GuncelleyenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WebSiteKasaHaraketleri_Kullanicilar_KaydedenKullaniciId",
                        column: x => x.KaydedenKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WebSiteKasaHaraketleri_WebSiteleri_WebSitesiId",
                        column: x => x.WebSitesiId,
                        principalTable: "WebSiteleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebSiteKasaHaraketleri_GuncelleyenKullaniciId",
                table: "WebSiteKasaHaraketleri",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_WebSiteKasaHaraketleri_KaydedenKullaniciId",
                table: "WebSiteKasaHaraketleri",
                column: "KaydedenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_WebSiteKasaHaraketleri_WebSitesiId",
                table: "WebSiteKasaHaraketleri",
                column: "WebSitesiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebSiteKasaHaraketleri");
        }
    }
}
