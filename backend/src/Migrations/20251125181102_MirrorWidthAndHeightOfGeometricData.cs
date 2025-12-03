using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MirrorWidthAndHeightOfGeometricData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double[]>(
                name: "Heights",
                schema: "database",
                table: "geometric_data",
                type: "double precision[]",
                nullable: false,
                defaultValue: System.Array.Empty<double>());

            migrationBuilder.AddColumn<double[]>(
                name: "Widths",
                schema: "database",
                table: "geometric_data",
                type: "double precision[]",
                nullable: false,
                defaultValue: System.Array.Empty<double>());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Heights",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "Widths",
                schema: "database",
                table: "geometric_data");
        }
    }
}
