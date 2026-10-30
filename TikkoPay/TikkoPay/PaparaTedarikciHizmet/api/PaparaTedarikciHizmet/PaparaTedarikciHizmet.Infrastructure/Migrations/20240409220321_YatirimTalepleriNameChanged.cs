using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaTedarikciHizmet.Infrastructure.Migrations
{
    public partial class YatirimTalepleriNameChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YatirimTalepleri_Bankalar_BankaId",
                table: "YatirimTalepleri");

            migrationBuilder.DropForeignKey(
                name: "FK_YatirimTalepleri_Kullanicilar_GuncelleyenKullaniciId",
                table: "YatirimTalepleri");

            migrationBuilder.DropForeignKey(
                name: "FK_YatirimTalepleri_Tedarikciler_TedarikciId",
                table: "YatirimTalepleri");

            migrationBuilder.DropForeignKey(
                name: "FK_YatirimTalepleri_WebSiteleri_WebSiteId",
                table: "YatirimTalepleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_YatirimTalepleri",
                table: "YatirimTalepleri");

            migrationBuilder.RenameTable(
                name: "YatirimTalepleri",
                newName: "YatirimCekimTalepleri");

            migrationBuilder.RenameIndex(
                name: "IX_YatirimTalepleri_WebSiteId",
                table: "YatirimCekimTalepleri",
                newName: "IX_YatirimCekimTalepleri_WebSiteId");

            migrationBuilder.RenameIndex(
                name: "IX_YatirimTalepleri_TedarikciId",
                table: "YatirimCekimTalepleri",
                newName: "IX_YatirimCekimTalepleri_TedarikciId");

            migrationBuilder.RenameIndex(
                name: "IX_YatirimTalepleri_GuncelleyenKullaniciId",
                table: "YatirimCekimTalepleri",
                newName: "IX_YatirimCekimTalepleri_GuncelleyenKullaniciId");

            migrationBuilder.RenameIndex(
                name: "IX_YatirimTalepleri_BankaId",
                table: "YatirimCekimTalepleri",
                newName: "IX_YatirimCekimTalepleri_BankaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_YatirimCekimTalepleri",
                table: "YatirimCekimTalepleri",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimCekimTalepleri_Bankalar_BankaId",
                table: "YatirimCekimTalepleri",
                column: "BankaId",
                principalTable: "Bankalar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimCekimTalepleri_Kullanicilar_GuncelleyenKullaniciId",
                table: "YatirimCekimTalepleri",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimCekimTalepleri_Tedarikciler_TedarikciId",
                table: "YatirimCekimTalepleri",
                column: "TedarikciId",
                principalTable: "Tedarikciler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimCekimTalepleri_WebSiteleri_WebSiteId",
                table: "YatirimCekimTalepleri",
                column: "WebSiteId",
                principalTable: "WebSiteleri",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YatirimCekimTalepleri_Bankalar_BankaId",
                table: "YatirimCekimTalepleri");

            migrationBuilder.DropForeignKey(
                name: "FK_YatirimCekimTalepleri_Kullanicilar_GuncelleyenKullaniciId",
                table: "YatirimCekimTalepleri");

            migrationBuilder.DropForeignKey(
                name: "FK_YatirimCekimTalepleri_Tedarikciler_TedarikciId",
                table: "YatirimCekimTalepleri");

            migrationBuilder.DropForeignKey(
                name: "FK_YatirimCekimTalepleri_WebSiteleri_WebSiteId",
                table: "YatirimCekimTalepleri");

            migrationBuilder.DropPrimaryKey(
                name: "PK_YatirimCekimTalepleri",
                table: "YatirimCekimTalepleri");

            migrationBuilder.RenameTable(
                name: "YatirimCekimTalepleri",
                newName: "YatirimTalepleri");

            migrationBuilder.RenameIndex(
                name: "IX_YatirimCekimTalepleri_WebSiteId",
                table: "YatirimTalepleri",
                newName: "IX_YatirimTalepleri_WebSiteId");

            migrationBuilder.RenameIndex(
                name: "IX_YatirimCekimTalepleri_TedarikciId",
                table: "YatirimTalepleri",
                newName: "IX_YatirimTalepleri_TedarikciId");

            migrationBuilder.RenameIndex(
                name: "IX_YatirimCekimTalepleri_GuncelleyenKullaniciId",
                table: "YatirimTalepleri",
                newName: "IX_YatirimTalepleri_GuncelleyenKullaniciId");

            migrationBuilder.RenameIndex(
                name: "IX_YatirimCekimTalepleri_BankaId",
                table: "YatirimTalepleri",
                newName: "IX_YatirimTalepleri_BankaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_YatirimTalepleri",
                table: "YatirimTalepleri",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimTalepleri_Bankalar_BankaId",
                table: "YatirimTalepleri",
                column: "BankaId",
                principalTable: "Bankalar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimTalepleri_Kullanicilar_GuncelleyenKullaniciId",
                table: "YatirimTalepleri",
                column: "GuncelleyenKullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimTalepleri_Tedarikciler_TedarikciId",
                table: "YatirimTalepleri",
                column: "TedarikciId",
                principalTable: "Tedarikciler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_YatirimTalepleri_WebSiteleri_WebSiteId",
                table: "YatirimTalepleri",
                column: "WebSiteId",
                principalTable: "WebSiteleri",
                principalColumn: "Id");
        }
    }
}
