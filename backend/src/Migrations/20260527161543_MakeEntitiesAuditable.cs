using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeEntitiesAuditable : Migration
    {
        /// <inheritdoc />
        [SuppressMessage("Performance", "CA1861")]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "user",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "user",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "photovoltaic_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(OffsetDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "photovoltaic_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "optical_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(OffsetDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "optical_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "lifeCycle_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(OffsetDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "lifeCycle_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "institution_access_rights",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "institution_access_rights",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "hygrothermal_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(OffsetDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "hygrothermal_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "get_https_resource",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "get_https_resource",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "geometric_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(OffsetDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "geometric_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "calorimetric_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(OffsetDateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "calorimetric_data",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.CreateIndex(
                name: "IX_user_CreatedAt_Id",
                schema: "database",
                table: "user",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_photovoltaic_data_CreatedAt_Id",
                schema: "database",
                table: "photovoltaic_data",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_optical_data_CreatedAt_Id",
                schema: "database",
                table: "optical_data",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lifeCycle_data_CreatedAt_Id",
                schema: "database",
                table: "lifeCycle_data",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_institution_access_rights_CreatedAt_Id",
                schema: "database",
                table: "institution_access_rights",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hygrothermal_data_CreatedAt_Id",
                schema: "database",
                table: "hygrothermal_data",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_CreatedAt_Id",
                schema: "database",
                table: "get_https_resource",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_geometric_data_CreatedAt_Id",
                schema: "database",
                table: "geometric_data",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_calorimetric_data_CreatedAt_Id",
                schema: "database",
                table: "calorimetric_data",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);
        }

        /// <inheritdoc />
        [SuppressMessage("Performance", "CA1861")]
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_CreatedAt_Id",
                schema: "database",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_photovoltaic_data_CreatedAt_Id",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropIndex(
                name: "IX_optical_data_CreatedAt_Id",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropIndex(
                name: "IX_lifeCycle_data_CreatedAt_Id",
                schema: "database",
                table: "lifeCycle_data");

            migrationBuilder.DropIndex(
                name: "IX_institution_access_rights_CreatedAt_Id",
                schema: "database",
                table: "institution_access_rights");

            migrationBuilder.DropIndex(
                name: "IX_hygrothermal_data_CreatedAt_Id",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_CreatedAt_Id",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_geometric_data_CreatedAt_Id",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropIndex(
                name: "IX_calorimetric_data_CreatedAt_Id",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "database",
                table: "user");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "user");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "lifeCycle_data");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "database",
                table: "institution_access_rights");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "institution_access_rights");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.AlterColumn<OffsetDateTime>(
                name: "CreatedAt",
                schema: "database",
                table: "photovoltaic_data",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<OffsetDateTime>(
                name: "CreatedAt",
                schema: "database",
                table: "optical_data",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<OffsetDateTime>(
                name: "CreatedAt",
                schema: "database",
                table: "lifeCycle_data",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<OffsetDateTime>(
                name: "CreatedAt",
                schema: "database",
                table: "hygrothermal_data",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<OffsetDateTime>(
                name: "CreatedAt",
                schema: "database",
                table: "geometric_data",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<OffsetDateTime>(
                name: "CreatedAt",
                schema: "database",
                table: "calorimetric_data",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");
        }
    }
}