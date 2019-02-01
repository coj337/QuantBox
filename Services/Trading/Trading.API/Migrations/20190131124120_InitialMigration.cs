using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trading.API.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbitrageResults",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    InitialCurrency = table.Column<string>(nullable: true),
                    EstimatedProfit = table.Column<decimal>(nullable: false),
                    ActualProfit = table.Column<decimal>(nullable: false),
                    Dust = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeResult",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Fees = table.Column<decimal>(nullable: false),
                    OrderSide = table.Column<int>(nullable: false),
                    MarketSymbol = table.Column<string>(nullable: true),
                    FillDate = table.Column<DateTime>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    AveragePrice = table.Column<decimal>(nullable: false),
                    TradeId = table.Column<string>(nullable: true),
                    AmountFilled = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Result = table.Column<int>(nullable: false),
                    OrderId = table.Column<string>(nullable: true),
                    FeesCurrency = table.Column<string>(nullable: true),
                    CorrelationId = table.Column<string>(nullable: true),
                    ArbitrageTradeResultsId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeResult_ArbitrageResults_ArbitrageTradeResultsId",
                        column: x => x.ArbitrageTradeResultsId,
                        principalTable: "ArbitrageResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeResult_ArbitrageTradeResultsId",
                table: "TradeResult",
                column: "ArbitrageTradeResultsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeResult");

            migrationBuilder.DropTable(
                name: "ArbitrageResults");
        }
    }
}
