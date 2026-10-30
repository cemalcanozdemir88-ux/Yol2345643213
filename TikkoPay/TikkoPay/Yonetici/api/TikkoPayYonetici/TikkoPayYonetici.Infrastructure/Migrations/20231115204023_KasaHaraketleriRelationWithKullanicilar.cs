using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TikkoPayYonetici.Infrastructure.Migrations
{
    public partial class KasaHaraketleriRelationWithKullanicilar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_KasaHaraketleri_GuncelleyenKullaniciId",
                table: "KasaHaraketleri",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KasaHaraketleri_KaydedenKullaniciId",
                table: "KasaHaraketleri",
                column: "KaydedenKullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_KasaHaraketleri_Kullanicilar_GuncelleyenKullaniciId",
                table: "KasaHaraketleri",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KasaHaraketleri_Kullanicilar_KaydedenKullaniciId",
                table: "KasaHaraketleri",
                column: "KaydedenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KasaHaraketleri_Kullanicilar_GuncelleyenKullaniciId",
                table: "KasaHaraketleri");

            migrationBuilder.DropForeignKey(
                name: "FK_KasaHaraketleri_Kullanicilar_KaydedenKullaniciId",
                table: "KasaHaraketleri");

            migrationBuilder.DropIndex(
                name: "IX_KasaHaraketleri_GuncelleyenKullaniciId",
                table: "KasaHaraketleri");

            migrationBuilder.DropIndex(
                name: "IX_KasaHaraketleri_KaydedenKullaniciId",
                table: "KasaHaraketleri");
        }
    }
}
