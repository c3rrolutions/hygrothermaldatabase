using System;
using System.Collections.Generic;
using Database.Data.AccessPolicies;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RevampAccessPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "institution_access_rights",
                schema: "database");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedInstitutions",
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
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "lifeCycle_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "lifeCycle_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedInstitutions",
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
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "lifeCycle_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.logical_combinator", "all,some");

            migrationBuilder.CreateTable(
                name: "institution_access_policy",
                schema: "database",
                columns: table => new
                {
                    InstitutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessCountSinceStartTime = table.Column<string>(type: "jsonb", nullable: true),
                    UpperAccessLimitPerTimeDuration = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution_access_policy", x => x.InstitutionId);
                });

            migrationBuilder.CreateTable(
                name: "open_id_connect_application_access_policy",
                schema: "database",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    AccessCountSinceStartTime = table.Column<string>(type: "jsonb", nullable: true),
                    UpperAccessLimitPerTimeDuration = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_open_id_connect_application_access_policy", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "user_access_policy",
                schema: "database",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessCountSinceStartTime = table.Column<string>(type: "jsonb", nullable: true),
                    UpperAccessLimitPerTimeDuration = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_access_policy", x => x.UserId);
                });

            migrationBuilder.AddColumn<Dictionary<string, object>>(
                name: "AccessPolicy",
                schema: "database",
                table: "photovoltaic_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, object>>(
                name: "AccessPolicy",
                schema: "database",
                table: "optical_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, object>>(
                name: "AccessPolicy",
                schema: "database",
                table: "lifeCycle_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, object>>(
                name: "AccessPolicy",
                schema: "database",
                table: "hygrothermal_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, object>>(
                name: "AccessPolicy",
                schema: "database",
                table: "geometric_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, object>>(
                name: "AccessPolicy",
                schema: "database",
                table: "calorimetric_data",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "institution_access_policy",
                schema: "database");

            migrationBuilder.DropTable(
                name: "open_id_connect_application_access_policy",
                schema: "database");

            migrationBuilder.DropTable(
                name: "user_access_policy",
                schema: "database");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "lifeCycle_data");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.AlterDatabase().OldAnnotation("Npgsql:Enum:database.logical_combinator", "all,some");

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "photovoltaic_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "optical_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "lifeCycle_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "hygrothermal_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "geometric_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<IReadOnlyDictionary<Guid, uint?>>(
                name: "DataAccessRights_AllowedUserAndQuantity",
                schema: "database",
                table: "calorimetric_data",
                type: "jsonb",
                nullable: true);

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

            migrationBuilder.AddColumn<string[]>(
                name: "DataAccessRights_AllowedApplications",
                schema: "database",
                table: "lifeCycle_data",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid[]>(
                name: "DataAccessRights_AllowedInstitutions",
                schema: "database",
                table: "lifeCycle_data",
                type: "uuid[]",
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

            migrationBuilder.CreateTable(
                name: "institution_access_rights",
                schema: "database",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    AllowedDatasetsPerTime = table.Column<long>(type: "bigint", nullable: true),
                    AllowedUserCount = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    InstitutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Period = table.Column<Duration>(type: "interval", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UserAlreadyAccessed = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_institution_access_rights", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_institution_access_rights_CreatedAt_Id",
                schema: "database",
                table: "institution_access_rights",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_institution_access_rights_InstitutionId",
                schema: "database",
                table: "institution_access_rights",
                column: "InstitutionId",
                unique: true);
        }
    }
}