using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TedarikciHizmet.Infrastructure.Migrations
{
    public partial class AddedKasaHaraketleri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KasaHaraketleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TedarikciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    UsdtMiktar = table.Column<decimal>(type: "decimal(13,4)", nullable: false),
                    UsdtAdres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UsdtTxtNo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IslemTipi = table.Column<int>(type: "int", nullable: false),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime", nullable: false),
                    KaydedenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime", nullable: true),
                    GuncelleyenKullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KasaHaraketleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KasaHaraketleri_Tedarikciler_TedarikciId",
                        column: x => x.TedarikciId,
                        principalTable: "Tedarikciler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KasaHaraketleri_TedarikciId",
                table: "KasaHaraketleri",
                column: "TedarikciId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KasaHaraketleri");
        }
    }
}
