using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TedarikciHizmet.Infrastructure.Migrations
{
    public partial class YatirimTalebiKullaniciRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_YatirimTalepleri_GuncelleyenKullaniciId",
                table: "YatirimTalepleri",
                column: "GuncelleyenKullaniciId");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimTalepleri_Kullanicilar_GuncelleyenKullaniciId",
                table: "YatirimTalepleri",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YatirimTalepleri_Kullanicilar_GuncelleyenKullaniciId",
                table: "YatirimTalepleri");

            migrationBuilder.DropIndex(
                name: "IX_YatirimTalepleri_GuncelleyenKullaniciId",
                table: "YatirimTalepleri");
        }
    }
}
