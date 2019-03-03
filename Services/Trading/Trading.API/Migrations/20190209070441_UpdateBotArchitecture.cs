using Microsoft.EntityFrameworkCore.Migrations;

namespace Trading.API.Migrations
{
    public partial class UpdateBotArchitecture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeCredentials_Bots_BotId",
                table: "ExchangeCredentials");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeCredentials_BotId",
                table: "ExchangeCredentials");

            migrationBuilder.DropColumn(
                name: "BotId",
                table: "ExchangeCredentials");

            migrationBuilder.CreateTable(
                name: "BotExchange",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SelectedConfigId = table.Column<string>(nullable: true),
                    BotId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotExchange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BotExchange_Bots_BotId",
                        column: x => x.BotId,
                        principalTable: "Bots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BotExchange_ExchangeCredentials_SelectedConfigId",
                        column: x => x.SelectedConfigId,
                        principalTable: "ExchangeCredentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotExchange_BotId",
                table: "BotExchange",
                column: "BotId");

            migrationBuilder.CreateIndex(
                name: "IX_BotExchange_SelectedConfigId",
                table: "BotExchange",
                column: "SelectedConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotExchange");

            migrationBuilder.AddColumn<string>(
                name: "BotId",
                table: "ExchangeCredentials",
                nullable: true);

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
    }
}
