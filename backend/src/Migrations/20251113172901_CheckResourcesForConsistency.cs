using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class CheckResourcesForConsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_calorimetric_data_CalorimetricDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_geometric_data_GeometricDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_hygrothermal_data_HygrothermalDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_optical_data_OpticalDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_CalorimetricDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_GeometricDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_HygrothermalDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_OpticalDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_PhotovoltaicDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "photovoltaic_data",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<JsonElement>(
                name: "Variables",
                schema: "database",
                table: "optical_data_Approvals",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{}'",
                oldClrType: typeof(JsonElement),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "optical_data",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<JsonElement>(
                name: "Approval_Variables",
                schema: "database",
                table: "optical_data",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'{}'",
                oldClrType: typeof(JsonElement),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "hygrothermal_data",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "geometric_data",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "calorimetric_data",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_CalorimetricDataId",
                schema: "database",
                table: "get_https_resource",
                column: "CalorimetricDataId",
                unique: true,
                filter: "\"CalorimetricDataId\" IS NOT NULL AND \"ParentId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_GeometricDataId",
                schema: "database",
                table: "get_https_resource",
                column: "GeometricDataId",
                unique: true,
                filter: "\"GeometricDataId\" IS NOT NULL AND \"ParentId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_HygrothermalDataId",
                schema: "database",
                table: "get_https_resource",
                column: "HygrothermalDataId",
                unique: true,
                filter: "\"HygrothermalDataId\" IS NOT NULL AND \"ParentId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_OpticalDataId",
                schema: "database",
                table: "get_https_resource",
                column: "OpticalDataId",
                unique: true,
                filter: "\"OpticalDataId\" IS NOT NULL AND \"ParentId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_PhotovoltaicDataId",
                schema: "database",
                table: "get_https_resource",
                column: "PhotovoltaicDataId",
                unique: true,
                filter: "\"PhotovoltaicDataId\" IS NOT NULL AND \"ParentId\" IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GetHttpsResource_Exactly_One_Data_Set",
                schema: "database",
                table: "get_https_resource",
                sql: "NUM_NONNULLS(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GetHttpsResource_Root_Or_Child",
                schema: "database",
                table: "get_https_resource",
                sql: "(\"ParentId\" IS NULL AND \"AppliedConversionMethod_MethodId\" IS NULL)\nOR (\"ParentId\" IS NOT NULL AND \"AppliedConversionMethod_MethodId\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_calorimetric_data_CalorimetricDataId",
                schema: "database",
                table: "get_https_resource",
                column: "CalorimetricDataId",
                principalSchema: "database",
                principalTable: "calorimetric_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_geometric_data_GeometricDataId",
                schema: "database",
                table: "get_https_resource",
                column: "GeometricDataId",
                principalSchema: "database",
                principalTable: "geometric_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_hygrothermal_data_HygrothermalDataId",
                schema: "database",
                table: "get_https_resource",
                column: "HygrothermalDataId",
                principalSchema: "database",
                principalTable: "hygrothermal_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_optical_data_OpticalDataId",
                schema: "database",
                table: "get_https_resource",
                column: "OpticalDataId",
                principalSchema: "database",
                principalTable: "optical_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId",
                schema: "database",
                table: "get_https_resource",
                column: "PhotovoltaicDataId",
                principalSchema: "database",
                principalTable: "photovoltaic_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_id_cannot_change\"() RETURNS trigger as $LC_TRIGGER_data_id_cannot_change$\r\nBEGIN\r\n  IF COALESCE(OLD.\"CalorimetricDataId\", OLD.\"GeometricDataId\", OLD.\"HygrothermalDataId\", OLD.\"OpticalDataId\", OLD.\"PhotovoltaicDataId\") <> COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\nTHEN\n    RAISE EXCEPTION 'You cannot change the data ID of a resource.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_id_cannot_change$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_id_cannot_change BEFORE UPDATE\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_id_cannot_change\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_ids_must_match\"() RETURNS trigger as $LC_TRIGGER_data_ids_must_match$\r\nBEGIN\r\n  IF NEW.\"ParentId\" IS NOT NULL\n   AND (\n       SELECT COUNT(\"Id\")\n       FROM database.\"get_https_resource\"\n       WHERE \"Id\" = NEW.\"ParentId\" AND COALESCE(\"CalorimetricDataId\", \"GeometricDataId\", \"HygrothermalDataId\", \"OpticalDataId\", \"PhotovoltaicDataId\") = COALESCE(NEW.\"CalorimetricDataId\", NEW.\"GeometricDataId\", NEW.\"HygrothermalDataId\", NEW.\"OpticalDataId\", NEW.\"PhotovoltaicDataId\")\n   )\n   <> 1\nTHEN\n    RAISE EXCEPTION 'The new resource must have the same data ID as its parent.';\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$LC_TRIGGER_data_ids_must_match$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_ids_must_match BEFORE INSERT\r\nON \"database\".\"get_https_resource\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_ids_must_match\"();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_id_cannot_change\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_ids_must_match\"() CASCADE;");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_calorimetric_data_CalorimetricDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_geometric_data_GeometricDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_hygrothermal_data_HygrothermalDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_optical_data_OpticalDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropForeignKey(
                name: "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_CalorimetricDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_GeometricDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_HygrothermalDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_OpticalDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropIndex(
                name: "IX_get_https_resource_PhotovoltaicDataId",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GetHttpsResource_Exactly_One_Data_Set",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GetHttpsResource_Root_Or_Child",
                schema: "database",
                table: "get_https_resource");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "photovoltaic_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<JsonElement>(
                name: "Variables",
                schema: "database",
                table: "optical_data_Approvals",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(JsonElement),
                oldType: "jsonb",
                oldDefaultValueSql: "'{}'");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "optical_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<JsonElement>(
                name: "Approval_Variables",
                schema: "database",
                table: "optical_data",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(JsonElement),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValueSql: "'{}'");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "hygrothermal_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "geometric_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "database",
                table: "calorimetric_data",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_CalorimetricDataId",
                schema: "database",
                table: "get_https_resource",
                column: "CalorimetricDataId");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_GeometricDataId",
                schema: "database",
                table: "get_https_resource",
                column: "GeometricDataId");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_HygrothermalDataId",
                schema: "database",
                table: "get_https_resource",
                column: "HygrothermalDataId");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_OpticalDataId",
                schema: "database",
                table: "get_https_resource",
                column: "OpticalDataId");

            migrationBuilder.CreateIndex(
                name: "IX_get_https_resource_PhotovoltaicDataId",
                schema: "database",
                table: "get_https_resource",
                column: "PhotovoltaicDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_calorimetric_data_CalorimetricDataId",
                schema: "database",
                table: "get_https_resource",
                column: "CalorimetricDataId",
                principalSchema: "database",
                principalTable: "calorimetric_data",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_geometric_data_GeometricDataId",
                schema: "database",
                table: "get_https_resource",
                column: "GeometricDataId",
                principalSchema: "database",
                principalTable: "geometric_data",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_hygrothermal_data_HygrothermalDataId",
                schema: "database",
                table: "get_https_resource",
                column: "HygrothermalDataId",
                principalSchema: "database",
                principalTable: "hygrothermal_data",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_optical_data_OpticalDataId",
                schema: "database",
                table: "get_https_resource",
                column: "OpticalDataId",
                principalSchema: "database",
                principalTable: "optical_data",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId",
                schema: "database",
                table: "get_https_resource",
                column: "PhotovoltaicDataId",
                principalSchema: "database",
                principalTable: "photovoltaic_data",
                principalColumn: "Id");
        }
    }
}
