using System;
using System.Text.Json;
using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Database.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "database");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:public.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

        migrationBuilder.CreateTable(
            name: "calorimetric_data",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                GValues = table.Column<double[]>(type: "double precision[]", nullable: false),
                UValues = table.Column<double[]>(type: "double precision[]", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                Locale = table.Column<string>(type: "text", nullable: false),
                ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                Description = table.Column<string>(type: "text", nullable: true),
                Warnings = table.Column<string[]>(type: "text[]", nullable: false),
                CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                AppliedMethod_MethodId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_calorimetric_data", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "hygrothermal_data",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                Locale = table.Column<string>(type: "text", nullable: false),
                ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                Description = table.Column<string>(type: "text", nullable: true),
                Warnings = table.Column<string[]>(type: "text[]", nullable: false),
                CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                AppliedMethod_MethodId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_hygrothermal_data", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "optical_data",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                NearnormalHemisphericalVisibleTransmittances = table.Column<double[]>(type: "double precision[]", nullable: false),
                NearnormalHemisphericalVisibleReflectances = table.Column<double[]>(type: "double precision[]", nullable: false),
                NearnormalHemisphericalSolarTransmittances = table.Column<double[]>(type: "double precision[]", nullable: false),
                NearnormalHemisphericalSolarReflectances = table.Column<double[]>(type: "double precision[]", nullable: false),
                InfraredEmittances = table.Column<double[]>(type: "double precision[]", nullable: false),
                ColorRenderingIndices = table.Column<double[]>(type: "double precision[]", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                Locale = table.Column<string>(type: "text", nullable: false),
                ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                Description = table.Column<string>(type: "text", nullable: true),
                Warnings = table.Column<string[]>(type: "text[]", nullable: false),
                CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                AppliedMethod_MethodId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_optical_data", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "photovoltaic_data",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                Locale = table.Column<string>(type: "text", nullable: false),
                ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                Description = table.Column<string>(type: "text", nullable: true),
                Warnings = table.Column<string[]>(type: "text[]", nullable: false),
                CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                AppliedMethod_MethodId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_photovoltaic_data", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "calorimetric_data_Approvals",
            schema: "database",
            columns: table => new
            {
                CalorimetricDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Signature = table.Column<string>(type: "text", nullable: false),
                KeyFingerprint = table.Column<string>(type: "text", nullable: false),
                Query = table.Column<string>(type: "text", nullable: false),
                Response = table.Column<string>(type: "text", nullable: false),
                ApproverId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_calorimetric_data_Approvals", x => new { x.CalorimetricDataId, x.Id });
                table.ForeignKey(
                    name: "FK_calorimetric_data_Approvals_calorimetric_data_CalorimetricD~",
                    column: x => x.CalorimetricDataId,
                    principalSchema: "database",
                    principalTable: "calorimetric_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "calorimetric_data_Arguments",
            schema: "database",
            columns: table => new
            {
                AppliedMethodCalorimetricDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<JsonDocument>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_calorimetric_data_Arguments", x => new { x.AppliedMethodCalorimetricDataId, x.Id });
                table.ForeignKey(
                    name: "FK_calorimetric_data_Arguments_calorimetric_data_AppliedMethod~",
                    column: x => x.AppliedMethodCalorimetricDataId,
                    principalSchema: "database",
                    principalTable: "calorimetric_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "calorimetric_data_Sources",
            schema: "database",
            columns: table => new
            {
                AppliedMethodCalorimetricDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value_DataId = table.Column<Guid>(type: "uuid", nullable: false),
                Value_DataTimestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Value_DataKind = table.Column<DataKind>(type: "data_kind", nullable: false),
                Value_DatabaseId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_calorimetric_data_Sources", x => new { x.AppliedMethodCalorimetricDataId, x.Id });
                table.ForeignKey(
                    name: "FK_calorimetric_data_Sources_calorimetric_data_AppliedMethodCa~",
                    column: x => x.AppliedMethodCalorimetricDataId,
                    principalSchema: "database",
                    principalTable: "calorimetric_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "hygrothermal_data_Approvals",
            schema: "database",
            columns: table => new
            {
                HygrothermalDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Signature = table.Column<string>(type: "text", nullable: false),
                KeyFingerprint = table.Column<string>(type: "text", nullable: false),
                Query = table.Column<string>(type: "text", nullable: false),
                Response = table.Column<string>(type: "text", nullable: false),
                ApproverId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_hygrothermal_data_Approvals", x => new { x.HygrothermalDataId, x.Id });
                table.ForeignKey(
                    name: "FK_hygrothermal_data_Approvals_hygrothermal_data_HygrothermalD~",
                    column: x => x.HygrothermalDataId,
                    principalSchema: "database",
                    principalTable: "hygrothermal_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "hygrothermal_data_Arguments",
            schema: "database",
            columns: table => new
            {
                AppliedMethodHygrothermalDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<JsonDocument>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_hygrothermal_data_Arguments", x => new { x.AppliedMethodHygrothermalDataId, x.Id });
                table.ForeignKey(
                    name: "FK_hygrothermal_data_Arguments_hygrothermal_data_AppliedMethod~",
                    column: x => x.AppliedMethodHygrothermalDataId,
                    principalSchema: "database",
                    principalTable: "hygrothermal_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "hygrothermal_data_Sources",
            schema: "database",
            columns: table => new
            {
                AppliedMethodHygrothermalDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value_DataId = table.Column<Guid>(type: "uuid", nullable: false),
                Value_DataTimestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Value_DataKind = table.Column<DataKind>(type: "data_kind", nullable: false),
                Value_DatabaseId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_hygrothermal_data_Sources", x => new { x.AppliedMethodHygrothermalDataId, x.Id });
                table.ForeignKey(
                    name: "FK_hygrothermal_data_Sources_hygrothermal_data_AppliedMethodHy~",
                    column: x => x.AppliedMethodHygrothermalDataId,
                    principalSchema: "database",
                    principalTable: "hygrothermal_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CielabColor",
            schema: "database",
            columns: table => new
            {
                OpticalDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                LStar = table.Column<double>(type: "double precision", nullable: false),
                AStar = table.Column<double>(type: "double precision", nullable: false),
                BStar = table.Column<double>(type: "double precision", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CielabColor", x => new { x.OpticalDataId, x.Id });
                table.ForeignKey(
                    name: "FK_CielabColor_optical_data_OpticalDataId",
                    column: x => x.OpticalDataId,
                    principalSchema: "database",
                    principalTable: "optical_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "optical_data_Approvals",
            schema: "database",
            columns: table => new
            {
                OpticalDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Signature = table.Column<string>(type: "text", nullable: false),
                KeyFingerprint = table.Column<string>(type: "text", nullable: false),
                Query = table.Column<string>(type: "text", nullable: false),
                Response = table.Column<string>(type: "text", nullable: false),
                ApproverId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_optical_data_Approvals", x => new { x.OpticalDataId, x.Id });
                table.ForeignKey(
                    name: "FK_optical_data_Approvals_optical_data_OpticalDataId",
                    column: x => x.OpticalDataId,
                    principalSchema: "database",
                    principalTable: "optical_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "optical_data_Arguments",
            schema: "database",
            columns: table => new
            {
                AppliedMethodOpticalDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<JsonDocument>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_optical_data_Arguments", x => new { x.AppliedMethodOpticalDataId, x.Id });
                table.ForeignKey(
                    name: "FK_optical_data_Arguments_optical_data_AppliedMethodOpticalDat~",
                    column: x => x.AppliedMethodOpticalDataId,
                    principalSchema: "database",
                    principalTable: "optical_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "optical_data_Sources",
            schema: "database",
            columns: table => new
            {
                AppliedMethodOpticalDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value_DataId = table.Column<Guid>(type: "uuid", nullable: false),
                Value_DataTimestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Value_DataKind = table.Column<DataKind>(type: "data_kind", nullable: false),
                Value_DatabaseId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_optical_data_Sources", x => new { x.AppliedMethodOpticalDataId, x.Id });
                table.ForeignKey(
                    name: "FK_optical_data_Sources_optical_data_AppliedMethodOpticalDataId",
                    column: x => x.AppliedMethodOpticalDataId,
                    principalSchema: "database",
                    principalTable: "optical_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "get_https_resource",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                Description = table.Column<string>(type: "text", nullable: false),
                HashValue = table.Column<string>(type: "text", nullable: false),
                DataFormatId = table.Column<Guid>(type: "uuid", nullable: false),
                CalorimetricDataId = table.Column<Guid>(type: "uuid", nullable: true),
                HygrothermalDataId = table.Column<Guid>(type: "uuid", nullable: true),
                OpticalDataId = table.Column<Guid>(type: "uuid", nullable: true),
                PhotovoltaicDataId = table.Column<Guid>(type: "uuid", nullable: true),
                ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                AppliedConversionMethod_MethodId = table.Column<Guid>(type: "uuid", nullable: true),
                AppliedConversionMethod_SourceName = table.Column<string>(type: "text", nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_get_https_resource", x => x.Id);
                table.ForeignKey(
                    name: "FK_get_https_resource_calorimetric_data_CalorimetricDataId",
                    column: x => x.CalorimetricDataId,
                    principalSchema: "database",
                    principalTable: "calorimetric_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_get_https_resource_get_https_resource_ParentId",
                    column: x => x.ParentId,
                    principalSchema: "database",
                    principalTable: "get_https_resource",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_get_https_resource_hygrothermal_data_HygrothermalDataId",
                    column: x => x.HygrothermalDataId,
                    principalSchema: "database",
                    principalTable: "hygrothermal_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_get_https_resource_optical_data_OpticalDataId",
                    column: x => x.OpticalDataId,
                    principalSchema: "database",
                    principalTable: "optical_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId",
                    column: x => x.PhotovoltaicDataId,
                    principalSchema: "database",
                    principalTable: "photovoltaic_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "photovoltaic_data_Approvals",
            schema: "database",
            columns: table => new
            {
                PhotovoltaicDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Signature = table.Column<string>(type: "text", nullable: false),
                KeyFingerprint = table.Column<string>(type: "text", nullable: false),
                Query = table.Column<string>(type: "text", nullable: false),
                Response = table.Column<string>(type: "text", nullable: false),
                ApproverId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_photovoltaic_data_Approvals", x => new { x.PhotovoltaicDataId, x.Id });
                table.ForeignKey(
                    name: "FK_photovoltaic_data_Approvals_photovoltaic_data_PhotovoltaicD~",
                    column: x => x.PhotovoltaicDataId,
                    principalSchema: "database",
                    principalTable: "photovoltaic_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "photovoltaic_data_Arguments",
            schema: "database",
            columns: table => new
            {
                AppliedMethodPhotovoltaicDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<JsonDocument>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_photovoltaic_data_Arguments", x => new { x.AppliedMethodPhotovoltaicDataId, x.Id });
                table.ForeignKey(
                    name: "FK_photovoltaic_data_Arguments_photovoltaic_data_AppliedMethod~",
                    column: x => x.AppliedMethodPhotovoltaicDataId,
                    principalSchema: "database",
                    principalTable: "photovoltaic_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "photovoltaic_data_Sources",
            schema: "database",
            columns: table => new
            {
                AppliedMethodPhotovoltaicDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value_DataId = table.Column<Guid>(type: "uuid", nullable: false),
                Value_DataTimestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                Value_DataKind = table.Column<DataKind>(type: "data_kind", nullable: false),
                Value_DatabaseId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_photovoltaic_data_Sources", x => new { x.AppliedMethodPhotovoltaicDataId, x.Id });
                table.ForeignKey(
                    name: "FK_photovoltaic_data_Sources_photovoltaic_data_AppliedMethodPh~",
                    column: x => x.AppliedMethodPhotovoltaicDataId,
                    principalSchema: "database",
                    principalTable: "photovoltaic_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "FileMetaInformation",
            schema: "database",
            columns: table => new
            {
                GetHttpsResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Path = table.Column<string[]>(type: "text[]", nullable: false),
                DataFormatId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FileMetaInformation", x => new { x.GetHttpsResourceId, x.Id });
                table.ForeignKey(
                    name: "FK_FileMetaInformation_get_https_resource_GetHttpsResourceId",
                    column: x => x.GetHttpsResourceId,
                    principalSchema: "database",
                    principalTable: "get_https_resource",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "get_https_resource_Arguments",
            schema: "database",
            columns: table => new
            {
                ToTreeVertexAppliedConversionMethodGetHttpsResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<JsonDocument>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_get_https_resource_Arguments", x => new { x.ToTreeVertexAppliedConversionMethodGetHttpsResourceId, x.Id });
                table.ForeignKey(
                    name: "FK_get_https_resource_Arguments_get_https_resource_ToTreeVerte~",
                    column: x => x.ToTreeVertexAppliedConversionMethodGetHttpsResourceId,
                    principalSchema: "database",
                    principalTable: "get_https_resource",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_get_https_resource_CalorimetricDataId",
            schema: "database",
            table: "get_https_resource",
            column: "CalorimetricDataId");

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
            name: "IX_get_https_resource_ParentId",
            schema: "database",
            table: "get_https_resource",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_get_https_resource_PhotovoltaicDataId",
            schema: "database",
            table: "get_https_resource",
            column: "PhotovoltaicDataId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "calorimetric_data_Approvals",
            schema: "database");

        migrationBuilder.DropTable(
            name: "calorimetric_data_Arguments",
            schema: "database");

        migrationBuilder.DropTable(
            name: "calorimetric_data_Sources",
            schema: "database");

        migrationBuilder.DropTable(
            name: "CielabColor",
            schema: "database");

        migrationBuilder.DropTable(
            name: "FileMetaInformation",
            schema: "database");

        migrationBuilder.DropTable(
            name: "get_https_resource_Arguments",
            schema: "database");

        migrationBuilder.DropTable(
            name: "hygrothermal_data_Approvals",
            schema: "database");

        migrationBuilder.DropTable(
            name: "hygrothermal_data_Arguments",
            schema: "database");

        migrationBuilder.DropTable(
            name: "hygrothermal_data_Sources",
            schema: "database");

        migrationBuilder.DropTable(
            name: "optical_data_Approvals",
            schema: "database");

        migrationBuilder.DropTable(
            name: "optical_data_Arguments",
            schema: "database");

        migrationBuilder.DropTable(
            name: "optical_data_Sources",
            schema: "database");

        migrationBuilder.DropTable(
            name: "photovoltaic_data_Approvals",
            schema: "database");

        migrationBuilder.DropTable(
            name: "photovoltaic_data_Arguments",
            schema: "database");

        migrationBuilder.DropTable(
            name: "photovoltaic_data_Sources",
            schema: "database");

        migrationBuilder.DropTable(
            name: "get_https_resource",
            schema: "database");

        migrationBuilder.DropTable(
            name: "calorimetric_data",
            schema: "database");

        migrationBuilder.DropTable(
            name: "hygrothermal_data",
            schema: "database");

        migrationBuilder.DropTable(
            name: "optical_data",
            schema: "database");

        migrationBuilder.DropTable(
            name: "photovoltaic_data",
            schema: "database");

        migrationBuilder.AlterDatabase()
            .OldAnnotation("Npgsql:Enum:public.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");
    }
}