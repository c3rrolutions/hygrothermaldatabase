using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "photovoltaic_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Query",
                schema: "database",
                table: "photovoltaic_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Response",
                schema: "database",
                table: "photovoltaic_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Signature",
                schema: "database",
                table: "photovoltaic_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Approval_Timestamp",
                schema: "database",
                table: "photovoltaic_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DataAccess",
                schema: "database",
                table: "photovoltaic_data",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "optical_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Query",
                schema: "database",
                table: "optical_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Response",
                schema: "database",
                table: "optical_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Signature",
                schema: "database",
                table: "optical_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Approval_Timestamp",
                schema: "database",
                table: "optical_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DataAccess",
                schema: "database",
                table: "optical_data",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "hygrothermal_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Query",
                schema: "database",
                table: "hygrothermal_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Response",
                schema: "database",
                table: "hygrothermal_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Signature",
                schema: "database",
                table: "hygrothermal_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Approval_Timestamp",
                schema: "database",
                table: "hygrothermal_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DataAccess",
                schema: "database",
                table: "hygrothermal_data",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "geometric_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Query",
                schema: "database",
                table: "geometric_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Response",
                schema: "database",
                table: "geometric_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Signature",
                schema: "database",
                table: "geometric_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Approval_Timestamp",
                schema: "database",
                table: "geometric_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DataAccess",
                schema: "database",
                table: "geometric_data",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "calorimetric_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Query",
                schema: "database",
                table: "calorimetric_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Response",
                schema: "database",
                table: "calorimetric_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approval_Signature",
                schema: "database",
                table: "calorimetric_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Approval_Timestamp",
                schema: "database",
                table: "calorimetric_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DataAccess",
                schema: "database",
                table: "calorimetric_data",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "access_rights",
                schema: "database",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    InstitutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowedUserCount = table.Column<long>(type: "bigint", nullable: true),
                    AllowedDatasetsPerTime = table.Column<long>(type: "bigint", nullable: true),
                    Period = table.Column<TimeSpan>(type: "interval", nullable: false),
                    UserOfInstitutionAlreadyAccessed = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_rights", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_rights",
                schema: "database");

            migrationBuilder.DropColumn(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "Approval_Query",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "Approval_Response",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "Approval_Signature",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "Approval_Timestamp",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "DataAccess",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "Approval_Query",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "Approval_Response",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "Approval_Signature",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "Approval_Timestamp",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "DataAccess",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "Approval_Query",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "Approval_Response",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "Approval_Signature",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "Approval_Timestamp",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "DataAccess",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "Approval_Query",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "Approval_Response",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "Approval_Signature",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "Approval_Timestamp",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "DataAccess",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "Approval_KeyFingerprint",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.DropColumn(
                name: "Approval_Query",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.DropColumn(
                name: "Approval_Response",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.DropColumn(
                name: "Approval_Signature",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.DropColumn(
                name: "Approval_Timestamp",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.DropColumn(
                name: "DataAccess",
                schema: "database",
                table: "calorimetric_data");
        }
    }
}
