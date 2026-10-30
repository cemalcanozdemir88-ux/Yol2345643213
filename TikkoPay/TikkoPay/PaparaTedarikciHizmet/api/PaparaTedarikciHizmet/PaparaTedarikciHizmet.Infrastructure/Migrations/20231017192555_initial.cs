using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaTedarikciHizmet.Infrastructure.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bankalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bankalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tedarikciler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Unvan = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BankaHavale = table.Column<bool>(type: "bit", nullable: false),
                    KalanLimit = table.Column<int>(type: "int", nullable: false),
                    IslemLimit = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    GercekBakiye = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    BakiyeLimit = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    YaririmKomisyonOrani = table.Column<double>(type: "float", nullable: false),
                    CekimKomisyonOrani = table.Column<double>(type: "float", nullable: false),
                    Durum = table.Column<bool>(type: "bit", nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tedarikciler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HavaleBankalari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TedarikciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HesapAdedi = table.Column<int>(type: "int", nullable: false),
                    HesapMinTutar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    HesapMaxTutar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    BnkHesapMinTutar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    BnkHesapMaxTutar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HavaleBankalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HavaleBankalari_Bankalar_BankaId",
                        column: x => x.BankaId,
                        principalTable: "Bankalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HavaleBankalari_Tedarikciler_TedarikciId",
                        column: x => x.TedarikciId,
                        principalTable: "Tedarikciler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TedarikciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Unvan = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    KullaniciTipi = table.Column<int>(type: "int", nullable: false),
                    TedarikciAnaKullanici = table.Column<bool>(type: "bit", nullable: false),
                    Durum = table.Column<bool>(type: "bit", nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Tedarikciler_TedarikciId",
                        column: x => x.TedarikciId,
                        principalTable: "Tedarikciler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YatirimTalepleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TedarikciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebSiteUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    YontemTipi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HesapSahibi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HesapNo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    Komisyon = table.Column<double>(type: "float", nullable: false),
                    Toplam = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    İslemTipi = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    OnayTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YatirimTalepleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YatirimTalepleri_Bankalar_BankaId",
                        column: x => x.BankaId,
                        principalTable: "Bankalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YatirimTalepleri_Tedarikciler_TedarikciId",
                        column: x => x.TedarikciId,
                        principalTable: "Tedarikciler",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HavaleBankalari_BankaId",
                table: "HavaleBankalari",
                column: "BankaId");

            migrationBuilder.CreateIndex(
                name: "IX_HavaleBankalari_TedarikciId",
                table: "HavaleBankalari",
                column: "TedarikciId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_TedarikciId",
                table: "Kullanicilar",
                column: "TedarikciId");

            migrationBuilder.CreateIndex(
                name: "IX_YatirimTalepleri_BankaId",
                table: "YatirimTalepleri",
                column: "BankaId");

            migrationBuilder.CreateIndex(
                name: "IX_YatirimTalepleri_TedarikciId",
                table: "YatirimTalepleri",
                column: "TedarikciId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HavaleBankalari");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "YatirimTalepleri");

            migrationBuilder.DropTable(
                name: "Bankalar");

            migrationBuilder.DropTable(
                name: "Tedarikciler");
        }
    }
}
