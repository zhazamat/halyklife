using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hbk.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnQuantityAndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "employee_market",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "employee_market",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "employee_market");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "employee_market");
        }
    }
}
