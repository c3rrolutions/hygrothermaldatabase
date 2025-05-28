using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDataAccessRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "photovoltaic_data",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid[]>(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "photovoltaic_data",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "photovoltaic_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "optical_data",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid[]>(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "optical_data",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "optical_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "hygrothermal_data",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid[]>(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "hygrothermal_data",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "hygrothermal_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "geometric_data",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid[]>(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "geometric_data",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "geometric_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "calorimetric_data",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid[]>(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "calorimetric_data",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "calorimetric_data",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "calorimetric_data");
        }
    }
}
