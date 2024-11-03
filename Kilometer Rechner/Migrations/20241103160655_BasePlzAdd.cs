using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kilometer_Rechner.Migrations
{
    /// <inheritdoc />
    public partial class BasePlzAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BasePlz",
                schema: "dbo",
                table: "Calculations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasePlz",
                schema: "dbo",
                table: "Calculations");
        }
    }
}
