using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trading.API.Migrations
{
    public partial class ChangeTimeTakenToStartEndTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeTaken",
                table: "ArbitrageResults");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeFinished",
                table: "ArbitrageResults",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStarted",
                table: "ArbitrageResults",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeFinished",
                table: "ArbitrageResults");

            migrationBuilder.DropColumn(
                name: "TimeStarted",
                table: "ArbitrageResults");

            migrationBuilder.AddColumn<long>(
                name: "TimeTaken",
                table: "ArbitrageResults",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
