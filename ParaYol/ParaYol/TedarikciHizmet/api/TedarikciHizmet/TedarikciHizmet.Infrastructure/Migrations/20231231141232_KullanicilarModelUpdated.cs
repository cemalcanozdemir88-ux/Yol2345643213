using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TedarikciHizmet.Infrastructure.Migrations
{
    public partial class KullanicilarModelUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Tedarikciler_TedarikciId",
                table: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "WebSiteKullanicilari");

            migrationBuilder.DropColumn(
                name: "AdminMi",
                table: "Tedarikciler");

            migrationBuilder.AlterColumn<Guid>(
                name: "TedarikciId",
                table: "Kullanicilar",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "WebSiteId",
                table: "Kullanicilar",
                type: "uniqueidentifier",
                nullable: true);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "WebSiteleriId",
            //    table: "Kullanicilar",
            //    type: "uniqueidentifier",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_WebSiteId",
                table: "Kullanicilar",
                column: "WebSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Tedarikciler_TedarikciId",
                table: "Kullanicilar",
                column: "TedarikciId",
                principalTable: "Tedarikciler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_WebSiteleri_WebSiteId",
                table: "Kullanicilar",
                column: "WebSiteId",
                principalTable: "WebSiteleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Tedarikciler_TedarikciId",
                table: "Kullanicilar");

            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_WebSiteleri_WebSiteId",
                table: "Kullanicilar");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_WebSiteId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "WebSiteId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "WebSiteId",
                table: "Kullanicilar");

            migrationBuilder.AddColumn<bool>(
                name: "AdminMi",
                table: "Tedarikciler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "TedarikciId",
                table: "Kullanicilar",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WebSiteKullanicilari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WebSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Durum = table.Column<bool>(type: "bit", nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KullaniciTipi = table.Column<int>(type: "int", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Tedarikciler_TedarikciId",
                table: "Kullanicilar",
                column: "TedarikciId",
                principalTable: "Tedarikciler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
