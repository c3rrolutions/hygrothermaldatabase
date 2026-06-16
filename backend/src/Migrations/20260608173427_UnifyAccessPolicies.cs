using System;
using System.Diagnostics.CodeAnalysis;
using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class UnifyAccessPolicies : Migration
    {
        /// <inheritdoc />
        [SuppressMessage("Performance", "CA1861")]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_id_cannot_change\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_ids_must_match\"() CASCADE;");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_access_policy",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_open_id_connect_application_access_policy",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_institution_access_policy",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessPolicy",
                schema: "database",
                table: "lifeCycle_data");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration",
                schema: "database",
                table: "institution_access_policy");

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

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "user_access_policy",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<long>(
                name: "AccessCountSinceStartTime_AccessCount",
                schema: "database",
                table: "user_access_policy",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AccessCountSinceStartTime_StartTime",
                schema: "database",
                table: "user_access_policy",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "user_access_policy",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<Guid>(
                name: "DataAccessPolicyId",
                schema: "database",
                table: "user_access_policy",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "user_access_policy",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "UpperAccessLimitPerTimeDuration_Duration",
                schema: "database",
                table: "user_access_policy",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpperAccessLimitPerTimeDuration_UpperLimit",
                schema: "database",
                table: "user_access_policy",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "database",
                table: "user_access_policy",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "user",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "photovoltaic_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "optical_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<long>(
                name: "AccessCountSinceStartTime_AccessCount",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AccessCountSinceStartTime_StartTime",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<Guid>(
                name: "DataAccessPolicyId",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "UpperAccessLimitPerTimeDuration_Duration",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpperAccessLimitPerTimeDuration_UpperLimit",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "lifeCycle_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "institution_access_policy",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<long>(
                name: "AccessCountSinceStartTime_AccessCount",
                schema: "database",
                table: "institution_access_policy",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AccessCountSinceStartTime_StartTime",
                schema: "database",
                table: "institution_access_policy",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "database",
                table: "institution_access_policy",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<Guid>(
                name: "DataAccessPolicyId",
                schema: "database",
                table: "institution_access_policy",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "database",
                table: "institution_access_policy",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "UpperAccessLimitPerTimeDuration_Duration",
                schema: "database",
                table: "institution_access_policy",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpperAccessLimitPerTimeDuration_UpperLimit",
                schema: "database",
                table: "institution_access_policy",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "database",
                table: "institution_access_policy",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "hygrothermal_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "get_https_resource",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "geometric_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "calorimetric_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuidv7()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_access_policy",
                schema: "database",
                table: "user_access_policy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_open_id_connect_application_access_policy",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_institution_access_policy",
                schema: "database",
                table: "institution_access_policy",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "data_access_policy",
                schema: "database",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    CalorimetricDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    GeometricDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    HygrothermalDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    LifeCycleDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    OpticalDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    PhotovoltaicDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    Combinator = table.Column<LogicalCombinator>(type: "database.logical_combinator", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_data_access_policy", x => x.Id);
                    table.CheckConstraint("CK_DataAccessPolicy_At_Most_One_Data_Set", "NUM_NONNULLS(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"LifeCycleDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") <= 1");
                    table.ForeignKey(
                        name: "FK_data_access_policy_calorimetric_data_CalorimetricDataId",
                        column: x => x.CalorimetricDataId,
                        principalSchema: "database",
                        principalTable: "calorimetric_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_data_access_policy_geometric_data_GeometricDataId",
                        column: x => x.GeometricDataId,
                        principalSchema: "database",
                        principalTable: "geometric_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_data_access_policy_hygrothermal_data_HygrothermalDataId",
                        column: x => x.HygrothermalDataId,
                        principalSchema: "database",
                        principalTable: "hygrothermal_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_data_access_policy_lifeCycle_data_LifeCycleDataId",
                        column: x => x.LifeCycleDataId,
                        principalSchema: "database",
                        principalTable: "lifeCycle_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_data_access_policy_optical_data_OpticalDataId",
                        column: x => x.OpticalDataId,
                        principalSchema: "database",
                        principalTable: "optical_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_data_access_policy_photovoltaic_data_PhotovoltaicDataId",
                        column: x => x.PhotovoltaicDataId,
                        principalSchema: "database",
                        principalTable: "photovoltaic_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_access_policy_CreatedAt_Id",
                schema: "database",
                table: "user_access_policy",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_access_policy_DataAccessPolicyId_UserId",
                schema: "database",
                table: "user_access_policy",
                columns: new[] { "DataAccessPolicyId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_open_id_connect_application_access_policy_CreatedAt_Id",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_open_id_connect_application_access_policy_DataAccessPolicyI~",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                columns: new[] { "DataAccessPolicyId", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_institution_access_policy_CreatedAt_Id",
                schema: "database",
                table: "institution_access_policy",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_institution_access_policy_DataAccessPolicyId_InstitutionId",
                schema: "database",
                table: "institution_access_policy",
                columns: new[] { "DataAccessPolicyId", "InstitutionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_data_access_policy_CalorimetricDataId",
                schema: "database",
                table: "data_access_policy",
                column: "CalorimetricDataId",
                unique: true,
                filter: "\"CalorimetricDataId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_data_access_policy_CalorimetricDataId_GeometricDataId_Hygro~",
                schema: "database",
                table: "data_access_policy",
                columns: new[] { "CalorimetricDataId", "GeometricDataId", "HygrothermalDataId", "LifeCycleDataId", "OpticalDataId", "PhotovoltaicDataId" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "IX_data_access_policy_CreatedAt_Id",
                schema: "database",
                table: "data_access_policy",
                columns: new[] { "CreatedAt", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_data_access_policy_GeometricDataId",
                schema: "database",
                table: "data_access_policy",
                column: "GeometricDataId",
                unique: true,
                filter: "\"GeometricDataId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_data_access_policy_HygrothermalDataId",
                schema: "database",
                table: "data_access_policy",
                column: "HygrothermalDataId",
                unique: true,
                filter: "\"HygrothermalDataId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_data_access_policy_LifeCycleDataId",
                schema: "database",
                table: "data_access_policy",
                column: "LifeCycleDataId",
                unique: true,
                filter: "\"LifeCycleDataId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_data_access_policy_OpticalDataId",
                schema: "database",
                table: "data_access_policy",
                column: "OpticalDataId",
                unique: true,
                filter: "\"OpticalDataId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_data_access_policy_PhotovoltaicDataId",
                schema: "database",
                table: "data_access_policy",
                column: "PhotovoltaicDataId",
                unique: true,
                filter: "\"PhotovoltaicDataId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_institution_access_policy_data_access_policy_DataAccessPoli~",
                schema: "database",
                table: "institution_access_policy",
                column: "DataAccessPolicyId",
                principalSchema: "database",
                principalTable: "data_access_policy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_open_id_connect_application_access_policy_data_access_polic~",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                column: "DataAccessPolicyId",
                principalSchema: "database",
                principalTable: "data_access_policy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_access_policy_data_access_policy_DataAccessPolicyId",
                schema: "database",
                table: "user_access_policy",
                column: "DataAccessPolicyId",
                principalSchema: "database",
                principalTable: "data_access_policy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_access_policy_data_id_cannot_change\"() RETURNS trigger as $LC_TRIGGER_data_access_policy_data_id_cannot_change$\r\nBEGIN\r\n  IF COALESCE(OLD.\"CalorimetricDataId\", OLD.\"GeometricDataId\", OLD.\"HygrothermalDataId\", OLD.\"LifeCycleDataId\", OLD.\"OpticalDataId\", OLD.\"PhotovoltaicDataId\") <> COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"LifeCycleDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\nTHEN\n    RAISE EXCEPTION 'You cannot change the data ID of a data access policy.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_access_policy_data_id_cannot_change$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_access_policy_data_id_cannot_change BEFORE UPDATE\r\nON \"database\".\"data_access_policy\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_access_policy_data_id_cannot_change\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_get_https_resource_data_id_cannot_change\"() RETURNS trigger as $LC_TRIGGER_get_https_resource_data_id_cannot_change$\r\nBEGIN\r\n  IF COALESCE(OLD.\"CalorimetricDataId\", OLD.\"GeometricDataId\", OLD.\"HygrothermalDataId\", OLD.\"LifeCycleDataId\", OLD.\"OpticalDataId\", OLD.\"PhotovoltaicDataId\") <> COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"LifeCycleDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\nTHEN\n    RAISE EXCEPTION 'You cannot change the data ID of a resource.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_get_https_resource_data_id_cannot_change$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_get_https_resource_data_id_cannot_change BEFORE UPDATE\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_get_https_resource_data_id_cannot_change\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_get_https_resource_data_ids_must_match\"() RETURNS trigger as $LC_TRIGGER_get_https_resource_data_ids_must_match$\r\nBEGIN\r\n  IF NEW.\"ParentId\" IS NOT NULL\n   AND (\n       SELECT COUNT(\"Id\")\n       FROM database.\"get_https_resource\"\n       WHERE \"Id\" = NEW.\"ParentId\" AND COALESCE(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"LifeCycleDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") = COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"LifeCycleDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\n   )\n   <> 1\nTHEN\n    RAISE EXCEPTION 'The new resource must have the same data ID as its parent.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_get_https_resource_data_ids_must_match$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_get_https_resource_data_ids_must_match BEFORE INSERT\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_get_https_resource_data_ids_must_match\"();");
        }

        /// <inheritdoc />
        [SuppressMessage("Performance", "CA1861")]
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_access_policy_data_id_cannot_change\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_get_https_resource_data_id_cannot_change\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_get_https_resource_data_ids_must_match\"() CASCADE;");

            migrationBuilder.DropForeignKey(
                name: "FK_institution_access_policy_data_access_policy_DataAccessPoli~",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropForeignKey(
                name: "FK_open_id_connect_application_access_policy_data_access_polic~",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropForeignKey(
                name: "FK_user_access_policy_data_access_policy_DataAccessPolicyId",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropTable(
                name: "data_access_policy",
                schema: "database");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_access_policy",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropIndex(
                name: "IX_user_access_policy_CreatedAt_Id",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropIndex(
                name: "IX_user_access_policy_DataAccessPolicyId_UserId",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_open_id_connect_application_access_policy",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropIndex(
                name: "IX_open_id_connect_application_access_policy_CreatedAt_Id",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropIndex(
                name: "IX_open_id_connect_application_access_policy_DataAccessPolicyI~",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_institution_access_policy",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropIndex(
                name: "IX_institution_access_policy_CreatedAt_Id",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropIndex(
                name: "IX_institution_access_policy_DataAccessPolicyId_InstitutionId",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime_AccessCount",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime_StartTime",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "DataAccessPolicyId",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration_Duration",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration_UpperLimit",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "database",
                table: "user_access_policy");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime_AccessCount",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime_StartTime",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "DataAccessPolicyId",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration_Duration",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration_UpperLimit",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "database",
                table: "open_id_connect_application_access_policy");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime_AccessCount",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "AccessCountSinceStartTime_StartTime",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "DataAccessPolicyId",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration_Duration",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "UpperAccessLimitPerTimeDuration_UpperLimit",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "database",
                table: "institution_access_policy");

            migrationBuilder.AddColumn<string>(
                name: "AccessCountSinceStartTime",
                schema: "database",
                table: "user_access_policy",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpperAccessLimitPerTimeDuration",
                schema: "database",
                table: "user_access_policy",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "user",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "photovoltaic_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<string>(
                name: "AccessPolicy",
                schema: "database",
                table: "photovoltaic_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "optical_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<string>(
                name: "AccessPolicy",
                schema: "database",
                table: "optical_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessCountSinceStartTime",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpperAccessLimitPerTimeDuration",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "lifeCycle_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<string>(
                name: "AccessPolicy",
                schema: "database",
                table: "lifeCycle_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessCountSinceStartTime",
                schema: "database",
                table: "institution_access_policy",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpperAccessLimitPerTimeDuration",
                schema: "database",
                table: "institution_access_policy",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "hygrothermal_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<string>(
                name: "AccessPolicy",
                schema: "database",
                table: "hygrothermal_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "get_https_resource",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "geometric_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<string>(
                name: "AccessPolicy",
                schema: "database",
                table: "geometric_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "database",
                table: "calorimetric_data",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuidv7()");

            migrationBuilder.AddColumn<string>(
                name: "AccessPolicy",
                schema: "database",
                table: "calorimetric_data",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_access_policy",
                schema: "database",
                table: "user_access_policy",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_open_id_connect_application_access_policy",
                schema: "database",
                table: "open_id_connect_application_access_policy",
                column: "ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_institution_access_policy",
                schema: "database",
                table: "institution_access_policy",
                column: "InstitutionId");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_id_cannot_change\"() RETURNS trigger as $LC_TRIGGER_data_id_cannot_change$\r\nBEGIN\r\n  IF COALESCE(OLD.\"CalorimetricDataId\", OLD.\"GeometricDataId\", OLD.\"HygrothermalDataId\", OLD.\"LifeCycleDataId\", OLD.\"OpticalDataId\", OLD.\"PhotovoltaicDataId\") <> COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"LifeCycleDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\nTHEN\n    RAISE EXCEPTION 'You cannot change the data ID of a resource.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_id_cannot_change$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_id_cannot_change BEFORE UPDATE\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_id_cannot_change\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_ids_must_match\"() RETURNS trigger as $LC_TRIGGER_data_ids_must_match$\r\nBEGIN\r\n  IF NEW.\"ParentId\" IS NOT NULL\n   AND (\n       SELECT COUNT(\"Id\")\n       FROM database.\"get_https_resource\"\n       WHERE \"Id\" = NEW.\"ParentId\" AND COALESCE(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"LifeCycleDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") = COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"LifeCycleDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\n   )\n   <> 1\nTHEN\n    RAISE EXCEPTION 'The new resource must have the same data ID as its parent.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_ids_must_match$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_ids_must_match BEFORE INSERT\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_ids_must_match\"();");
        }
    }
}