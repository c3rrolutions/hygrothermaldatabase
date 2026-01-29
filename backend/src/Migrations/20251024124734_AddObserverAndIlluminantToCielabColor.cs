using System.Text.Json;
using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddObserverAndIlluminantToCielabColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.calorimetric_observer", "ten_degrees,two_degrees")
                .Annotation("Npgsql:Enum:database.illuminant", "a,d65");

            migrationBuilder.AlterColumn<JsonElement>(
                name: "Variables",
                schema: "database",
                table: "optical_data_Approvals",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(JsonElement),
                oldType: "jsonb",
                oldDefaultValueSql: "'{}'");

            migrationBuilder.AlterColumn<JsonElement>(
                name: "Approval_Variables",
                schema: "database",
                table: "optical_data",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(JsonElement),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValueSql: "'{}'");

            migrationBuilder.AddColumn<Illuminant>(
                name: "Illuminant",
                schema: "database",
                table: "CielabColor",
                type: "database.illuminant",
                nullable: true);

            migrationBuilder.AddColumn<CalorimetricObserver>(
                name: "Observer",
                schema: "database",
                table: "CielabColor",
                type: "database.calorimetric_observer",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_OpticalData_CielabColors_LStar",
                schema: "database",
                table: "CielabColor",
                sql: "\"LStar\" >= 0.0 AND \"LStar\" <= 100.0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OpticalData_CielabColors_LStar",
                schema: "database",
                table: "CielabColor");

            migrationBuilder.DropColumn(
                name: "Illuminant",
                schema: "database",
                table: "CielabColor");

            migrationBuilder.DropColumn(
                name: "Observer",
                schema: "database",
                table: "CielabColor");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:database.calorimetric_observer", "ten_degrees,two_degrees")
                .OldAnnotation("Npgsql:Enum:database.illuminant", "a,d65");

            migrationBuilder.AlterColumn<JsonElement>(
                name: "Variables",
                schema: "database",
                table: "optical_data_Approvals",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'",
                oldClrType: typeof(JsonElement),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<JsonElement>(
                name: "Approval_Variables",
                schema: "database",
                table: "optical_data",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'{}'",
                oldClrType: typeof(JsonElement),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}