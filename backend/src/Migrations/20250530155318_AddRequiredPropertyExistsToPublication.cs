using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class AddRequiredPropertyExistsToPublication : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "boolean",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "optical_data_Approvals",
            type: "boolean",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "boolean",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "geometric_data_Approvals",
            type: "boolean",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "boolean",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "geometric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Statement_Publication_Exists",
            schema: "database",
            table: "calorimetric_data_Approvals");
    }
}
