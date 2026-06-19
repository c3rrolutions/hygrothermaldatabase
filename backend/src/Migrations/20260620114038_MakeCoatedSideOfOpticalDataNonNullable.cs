using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeCoatedSideOfOpticalDataNonNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<CoatedSide>(
                name: "CoatedSide",
                schema: "database",
                table: "optical_data",
                type: "database.coated_side",
                nullable: false,
                defaultValue: CoatedSide.UNKNOWN,
                oldClrType: typeof(CoatedSide),
                oldType: "database.coated_side",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<CoatedSide>(
                name: "CoatedSide",
                schema: "database",
                table: "optical_data",
                type: "database.coated_side",
                nullable: true,
                oldClrType: typeof(CoatedSide),
                oldType: "database.coated_side");
        }
    }
}