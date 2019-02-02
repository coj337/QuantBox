using Microsoft.EntityFrameworkCore.Migrations;

namespace Trading.API.Migrations
{
    public partial class RemoveExtraExchangeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceTrackingEnabled",
                table: "Exchanges");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PriceTrackingEnabled",
                table: "Exchanges",
                nullable: false,
                defaultValue: false);
        }
    }
}
