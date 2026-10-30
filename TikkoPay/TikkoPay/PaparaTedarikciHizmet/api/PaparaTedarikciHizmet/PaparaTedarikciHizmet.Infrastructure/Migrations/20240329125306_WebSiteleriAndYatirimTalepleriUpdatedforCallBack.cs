using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaparaTedarikciHizmet.Infrastructure.Migrations
{
    public partial class WebSiteleriAndYatirimTalepleriUpdatedforCallBack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSendToWebSite",
                table: "YatirimTalepleri",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WebSiteCallBackResult",
                table: "YatirimTalepleri",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositCallbackUrl",
                table: "WebSiteleri",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WithdrawCallbackUrl",
                table: "WebSiteleri",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSendToWebSite",
                table: "YatirimTalepleri");

            migrationBuilder.DropColumn(
                name: "WebSiteCallBackResult",
                table: "YatirimTalepleri");

            migrationBuilder.DropColumn(
                name: "DepositCallbackUrl",
                table: "WebSiteleri");

            migrationBuilder.DropColumn(
                name: "WithdrawCallbackUrl",
                table: "WebSiteleri");
        }
    }
}
