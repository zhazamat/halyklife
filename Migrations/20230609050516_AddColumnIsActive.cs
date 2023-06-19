using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hbk.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "thanksboard",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "thanksboard");
        }
    }
}
