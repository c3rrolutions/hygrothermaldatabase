using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class AddQueryVariablesToApprovals : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<JsonElement>(
            name: "Variables",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "jsonb",
            nullable: false,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Approval_Variables",
            schema: "database",
            table: "photovoltaic_data",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Variables",
            schema: "database",
            table: "optical_data_Approvals",
            type: "jsonb",
            nullable: false,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Approval_Variables",
            schema: "database",
            table: "optical_data",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Variables",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "jsonb",
            nullable: false,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Approval_Variables",
            schema: "database",
            table: "hygrothermal_data",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Variables",
            schema: "database",
            table: "geometric_data_Approvals",
            type: "jsonb",
            nullable: false,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Approval_Variables",
            schema: "database",
            table: "geometric_data",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Variables",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "jsonb",
            nullable: false,
            defaultValueSql: "'{}'");

        migrationBuilder.AddColumn<JsonElement>(
            name: "Approval_Variables",
            schema: "database",
            table: "calorimetric_data",
            type: "jsonb",
            nullable: true,
            defaultValueSql: "'{}'");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Variables",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Approval_Variables",
            schema: "database",
            table: "photovoltaic_data");

        migrationBuilder.DropColumn(
            name: "Variables",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Approval_Variables",
            schema: "database",
            table: "optical_data");

        migrationBuilder.DropColumn(
            name: "Variables",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Approval_Variables",
            schema: "database",
            table: "hygrothermal_data");

        migrationBuilder.DropColumn(
            name: "Variables",
            schema: "database",
            table: "geometric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Approval_Variables",
            schema: "database",
            table: "geometric_data");

        migrationBuilder.DropColumn(
            name: "Variables",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Approval_Variables",
            schema: "database",
            table: "calorimetric_data");
    }
}
