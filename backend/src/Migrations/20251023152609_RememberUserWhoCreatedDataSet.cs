using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RememberUserWhoCreatedDataSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "photovoltaic_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "optical_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "hygrothermal_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "geometric_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "calorimetric_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "database",
                table: "calorimetric_data");
        }
    }
}
