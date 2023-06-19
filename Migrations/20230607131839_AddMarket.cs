using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace hbk.Migrations
{
    /// <inheritdoc />
    public partial class AddMarket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_thanksboard_categories_CategoryId",
                table: "thanksboard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employee_market",
                table: "employee_market");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "markets");

            migrationBuilder.RenameColumn(
                name: "GiftImg",
                table: "markets",
                newName: "ProductCategory");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "thanksboard",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "markets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "markets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "employee_market",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_employee_market",
                table: "employee_market",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_market_EmployeeId",
                table: "employee_market",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_thanksboard_categories_CategoryId",
                table: "thanksboard",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_thanksboard_categories_CategoryId",
                table: "thanksboard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employee_market",
                table: "employee_market");

            migrationBuilder.DropIndex(
                name: "IX_employee_market_EmployeeId",
                table: "employee_market");

            migrationBuilder.RenameColumn(
                name: "ProductCategory",
                table: "markets",
                newName: "GiftImg");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "thanksboard",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "markets",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "markets",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "markets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "employee_market",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_employee_market",
                table: "employee_market",
                columns: new[] { "EmployeeId", "MarketId" });

            migrationBuilder.AddForeignKey(
                name: "FK_thanksboard_categories_CategoryId",
                table: "thanksboard",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
