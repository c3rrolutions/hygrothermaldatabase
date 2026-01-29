using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class RenameXminToVersion : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "Value_DataTimestamp",
            schema: "database",
            table: "photovoltaic_data_Sources",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Timestamp",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            schema: "database",
            table: "photovoltaic_data",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Value_DataTimestamp",
            schema: "database",
            table: "optical_data_Sources",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Timestamp",
            schema: "database",
            table: "optical_data_Approvals",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            schema: "database",
            table: "optical_data",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Value_DataTimestamp",
            schema: "database",
            table: "hygrothermal_data_Sources",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Timestamp",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            schema: "database",
            table: "hygrothermal_data",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Value_DataTimestamp",
            schema: "database",
            table: "calorimetric_data_Sources",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Timestamp",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            schema: "database",
            table: "calorimetric_data",
            type: "timestamp with time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp without time zone");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "Value_DataTimestamp",
            schema: "database",
            table: "photovoltaic_data_Sources",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Timestamp",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            schema: "database",
            table: "photovoltaic_data",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Value_DataTimestamp",
            schema: "database",
            table: "optical_data_Sources",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Timestamp",
            schema: "database",
            table: "optical_data_Approvals",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            schema: "database",
            table: "optical_data",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Value_DataTimestamp",
            schema: "database",
            table: "hygrothermal_data_Sources",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Timestamp",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            schema: "database",
            table: "hygrothermal_data",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Value_DataTimestamp",
            schema: "database",
            table: "calorimetric_data_Sources",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "Timestamp",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            schema: "database",
            table: "calorimetric_data",
            type: "timestamp without time zone",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "timestamp with time zone");
    }
}