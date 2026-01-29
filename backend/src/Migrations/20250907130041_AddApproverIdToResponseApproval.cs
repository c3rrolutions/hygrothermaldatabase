using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddApproverIdToResponseApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Approval_ApproverId",
                schema: "database",
                table: "photovoltaic_data",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Approval_ApproverId",
                schema: "database",
                table: "optical_data",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Approval_ApproverId",
                schema: "database",
                table: "hygrothermal_data",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Approval_ApproverId",
                schema: "database",
                table: "geometric_data",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Approval_ApproverId",
                schema: "database",
                table: "calorimetric_data",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approval_ApproverId",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "Approval_ApproverId",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "Approval_ApproverId",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "Approval_ApproverId",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "Approval_ApproverId",
                schema: "database",
                table: "calorimetric_data");
        }
    }
}