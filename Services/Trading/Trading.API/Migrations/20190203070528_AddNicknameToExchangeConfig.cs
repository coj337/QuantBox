using Microsoft.EntityFrameworkCore.Migrations;

namespace Trading.API.Migrations
{
    public partial class AddNicknameToExchangeConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "ExchangeCredentials",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "ExchangeCredentials");
        }
    }
}
