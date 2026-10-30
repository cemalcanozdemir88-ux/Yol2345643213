using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParaYolYonetici.Infrastructure.Migrations
{
    public partial class PasswordHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Sifre",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Kullanicilar",
                type: "varbinary(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Kullanicilar",
                type: "varbinary(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Kullanicilar");

            migrationBuilder.AlterColumn<string>(
                name: "Sifre",
                table: "Kullanicilar",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
