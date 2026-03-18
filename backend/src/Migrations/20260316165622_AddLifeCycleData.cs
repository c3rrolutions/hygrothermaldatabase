using System;
using System.Collections.Generic;
using System.Text.Json;
using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLifeCycleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_id_cannot_change\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_ids_must_match\"() CASCADE;");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GetHttpsResource_Exactly_One_Data_Set",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.data_kind", "calorimetric_data,geometric_data,hygrothermal_data,life_cycle_data,optical_data,photovoltaic_data")
                .OldAnnotation("Npgsql:Enum:database.data_kind", "calorimetric_data,geometric_data,hygrothermal_data,optical_data,photovoltaic_data");

            migrationBuilder.AddColumn<Guid>(
                name: "LifeCycleDataId",
                schema: "database",
                table: "get_https_resource",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "lifeCycle_data",
                schema: "database",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Locale = table.Column<string>(type: "text", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Warnings = table.Column<string[]>(type: "text[]", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<OffsetDateTime>(type: "timestamp with time zone", nullable: false),
                    AppliedMethod_MethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Approval_Timestamp = table.Column<OffsetDateTime>(type: "timestamp with time zone", nullable: true),
                    Approval_Signature = table.Column<string>(type: "text", nullable: true),
                    Approval_KeyFingerprint = table.Column<string>(type: "text", nullable: true),
                    Approval_Query = table.Column<string>(type: "text", nullable: true),
                    Approval_Variables = table.Column<JsonElement>(type: "jsonb", nullable: true, defaultValueSql: "'{}'"),
                    Approval_Message = table.Column<string>(type: "text", nullable: true),
                    Approval_ApproverId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataAccessRights_AllowedUserAndQuantity = table.Column<IReadOnlyDictionary<Guid, uint?>>(type: "jsonb", nullable: true),
                    DataAccessRights_AllowedInstitutions = table.Column<Guid[]>(type: "uuid[]", nullable: true),
                    DataAccessRights_AllowedApplications = table.Column<string[]>(type: "text[]", nullable: true),
                    PublishingState = table.Column<PublishingState>(type: "database.publishing_state", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lifeCycle_data", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lifeCycle_data_Approvals",
                schema: "database",
                columns: table => new
                {
                    LifeCycleDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApproverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<OffsetDateTime>(type: "timestamp with time zone", nullable: false),
                    Signature = table.Column<string>(type: "text", nullable: false),
                    KeyFingerprint = table.Column<string>(type: "text", nullable: false),
                    Query = table.Column<string>(type: "text", nullable: false),
                    Variables = table.Column<JsonElement>(type: "jsonb", nullable: false, defaultValueSql: "'{}'"),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Statement_Standard_Year = table.Column<int>(type: "integer", nullable: true),
                    Statement_Standard_Numeration_Prefix = table.Column<string>(type: "text", nullable: true),
                    Statement_Standard_Numeration_MainNumber = table.Column<string>(type: "text", nullable: true),
                    Statement_Standard_Numeration_Suffix = table.Column<string>(type: "text", nullable: true),
                    Statement_Standard_Standardizers = table.Column<Standardizer[]>(type: "database.standardizer[]", nullable: true),
                    Statement_Standard_Locator = table.Column<string>(type: "text", nullable: true),
                    Statement_Standard_Title = table.Column<string>(type: "text", nullable: true),
                    Statement_Standard_Abstract = table.Column<string>(type: "text", nullable: true),
                    Statement_Standard_Section = table.Column<string>(type: "text", nullable: true),
                    Statement_Publication_Authors = table.Column<string[]>(type: "text[]", nullable: true),
                    Statement_Publication_Doi = table.Column<string>(type: "text", nullable: true),
                    Statement_Publication_ArXiv = table.Column<string>(type: "text", nullable: true),
                    Statement_Publication_Urn = table.Column<string>(type: "text", nullable: true),
                    Statement_Publication_WebAddress = table.Column<string>(type: "text", nullable: true),
                    Statement_Publication_Title = table.Column<string>(type: "text", nullable: true),
                    Statement_Publication_Abstract = table.Column<string>(type: "text", nullable: true),
                    Statement_Publication_Section = table.Column<string>(type: "text", nullable: true),
                    Statement_Publication_Exists = table.Column<bool>(type: "boolean", nullable: true),
                    Statement_Exists = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lifeCycle_data_Approvals", x => new { x.LifeCycleDataId, x.Id });
                    table.ForeignKey(
                        name: "FK_lifeCycle_data_Approvals_lifeCycle_data_LifeCycleDataId",
                        column: x => x.LifeCycleDataId,
                        principalSchema: "database",
                        principalTable: "lifeCycle_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lifeCycle_data_Arguments",
                schema: "database",
                columns: table => new
                {
                    AppliedMethodLifeCycleDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<JsonElement>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lifeCycle_data_Arguments", x => new { x.AppliedMethodLifeCycleDataId, x.Id });
                    table.ForeignKey(
                        name: "FK_lifeCycle_data_Arguments_lifeCycle_data_AppliedMethodLifeCy~",
                        column: x => x.AppliedMethodLifeCycleDataId,
                        principalSchema: "database",
                        principalTable: "lifeCycle_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lifeCycle_data_Sources",
                schema: "database",
                columns: table => new
                {
                    AppliedMethodLifeCycleDataId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value_DataId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value_DataTimestamp = table.Column<OffsetDateTime>(type: "timestamp with time zone", nullable: false),
                    Value_DataKind = table.Column<DataKind>(type: "database.data_kind", nullable: false),
                    Value_DatabaseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lifeCycle_data_Sources", x => new { x.AppliedMethodLifeCycleDataId, x.Id });
                    table.ForeignKey(
                        name: "FK_lifeCycle_data_Sources_lifeCycle_data_AppliedMethodLifeCycl~",
                        column: x => x.AppliedMethodLifeCycleDataId,
                        principalSchema: "database",
                        principalTable: "lifeCycle_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_LifeCycleDataId",
                schema: "database",
                table: "get_https_resource",
                column: "LifeCycleDataId",
                unique: true,
                filter: "\"LifeCycleDataId\" IS NOT NULL AND \"ParentId\" IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GetHttpsResource_Exactly_One_Data_Set",
                schema: "database",
                table: "get_https_resource",
                sql: "NUM_NONNULLS(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"LifeCycleDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_lifeCycle_data_LifeCycleDataId",
                schema: "database",
                table: "get_https_resource",
                column: "LifeCycleDataId",
                principalSchema: "database",
                principalTable: "lifeCycle_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_id_cannot_change\"() RETURNS trigger as $LC_TRIGGER_data_id_cannot_change$\r\nBEGIN\r\n  IF COALESCE(OLD.\"CalorimetricDataId\", OLD.\"GeometricDataId\", OLD.\"HygrothermalDataId\", OLD.\"LifeCycleDataId\", OLD.\"OpticalDataId\", OLD.\"PhotovoltaicDataId\") <> COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"LifeCycleDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\nTHEN\n    RAISE EXCEPTION 'You cannot change the data ID of a resource.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_id_cannot_change$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_id_cannot_change BEFORE UPDATE\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_id_cannot_change\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_ids_must_match\"() RETURNS trigger as $LC_TRIGGER_data_ids_must_match$\r\nBEGIN\r\n  IF NEW.\"ParentId\" IS NOT NULL\n   AND (\n       SELECT COUNT(\"Id\")\n       FROM database.\"get_https_resource\"\n       WHERE \"Id\" = NEW.\"ParentId\" AND COALESCE(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"LifeCycleDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") = COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"LifeCycleDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\n   )\n   <> 1\nTHEN\n    RAISE EXCEPTION 'The new resource must have the same data ID as its parent.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_ids_must_match$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_ids_must_match BEFORE INSERT\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_ids_must_match\"();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_id_cannot_change\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_ids_must_match\"() CASCADE;");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_lifeCycle_data_LifeCycleDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropTable(
                name: "lifeCycle_data_Approvals",
                schema: "database");

            migrationBuilder.DropTable(
                name: "lifeCycle_data_Arguments",
                schema: "database");

            migrationBuilder.DropTable(
                name: "lifeCycle_data_Sources",
                schema: "database");

            migrationBuilder.DropTable(
                name: "lifeCycle_data",
                schema: "database");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_LifeCycleDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GetHttpsResource_Exactly_One_Data_Set",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropColumn(
                name: "LifeCycleDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.data_kind", "calorimetric_data,geometric_data,hygrothermal_data,optical_data,photovoltaic_data")
                .OldAnnotation("Npgsql:Enum:database.data_kind", "calorimetric_data,geometric_data,hygrothermal_data,life_cycle_data,optical_data,photovoltaic_data");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GetHttpsResource_Exactly_One_Data_Set",
                schema: "database",
                table: "get_https_resource",
                sql: "NUM_NONNULLS(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") = 1");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_id_cannot_change\"() RETURNS trigger as $LC_TRIGGER_data_id_cannot_change$\r\nBEGIN\r\n  IF COALESCE(OLD.\"CalorimetricDataId\", OLD.\"GeometricDataId\", OLD.\"HygrothermalDataId\", OLD.\"OpticalDataId\", OLD.\"PhotovoltaicDataId\") <> COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\nTHEN\n    RAISE EXCEPTION 'You cannot change the data ID of a resource.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_id_cannot_change$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_id_cannot_change BEFORE UPDATE\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_id_cannot_change\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_ids_must_match\"() RETURNS trigger as $LC_TRIGGER_data_ids_must_match$\r\nBEGIN\r\n  IF NEW.\"ParentId\" IS NOT NULL\n   AND (\n       SELECT COUNT(\"Id\")\n       FROM database.\"get_https_resource\"\n       WHERE \"Id\" = NEW.\"ParentId\" AND COALESCE(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") = COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\n   )\n   <> 1\nTHEN\n    RAISE EXCEPTION 'The new resource must have the same data ID as its parent.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_ids_must_match$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_ids_must_match BEFORE INSERT\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_ids_must_match\"();");
        }
    }
}