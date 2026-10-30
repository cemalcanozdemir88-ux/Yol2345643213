using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikkoPayYonetici.Infrastructure.Migrations
{
    public partial class YetkilerCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Yetkiler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UstId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SiraNumarasi = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yetkiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciYetkileri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YetkiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciYetkileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciYetkileri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciYetkileri_Yetkiler_YetkiId",
                        column: x => x.YetkiId,
                        principalTable: "Yetkiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciYetkileri_KullaniciId",
                table: "KullaniciYetkileri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciYetkileri_YetkiId",
                table: "KullaniciYetkileri",
                column: "YetkiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciYetkileri");

            migrationBuilder.DropTable(
                name: "Yetkiler");
        }
    }
}
