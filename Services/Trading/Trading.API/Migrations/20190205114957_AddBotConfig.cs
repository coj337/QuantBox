using Microsoft.EntityFrameworkCore.Migrations;

namespace Trading.API.Migrations
{
    public partial class AddBotConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BotId",
                table: "ExchangeCredentials",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TradingEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeCredentials_BotId",
                table: "ExchangeCredentials",
                column: "BotId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeCredentials_Bots_BotId",
                table: "ExchangeCredentials",
                column: "BotId",
                principalTable: "Bots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeCredentials_Bots_BotId",
                table: "ExchangeCredentials");

            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeCredentials_BotId",
                table: "ExchangeCredentials");

            migrationBuilder.DropColumn(
                name: "BotId",
                table: "ExchangeCredentials");
        }
    }
}
