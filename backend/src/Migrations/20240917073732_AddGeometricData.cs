using System;
using System.Text.Json;
using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class AddGeometricData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:database.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data,geometric_data")
            .OldAnnotation("Npgsql:Enum:database.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data");

        migrationBuilder.AddColumn<Guid>(
            name: "GeometricDataId",
            schema: "database",
            table: "get_https_resource",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "geometric_data",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                Thicknesses = table.Column<double[]>(type: "double precision[]", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                Locale = table.Column<string>(type: "text", nullable: false),
                ComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                Description = table.Column<string>(type: "text", nullable: true),
                Warnings = table.Column<string[]>(type: "text[]", nullable: false),
                CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                AppliedMethod_MethodId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_geometric_data", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "geometric_data_Approvals",
            schema: "database",
            columns: table => new
            {
                GeometricDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ApproverId = table.Column<Guid>(type: "uuid", nullable: false),
                Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Signature = table.Column<string>(type: "text", nullable: false),
                KeyFingerprint = table.Column<string>(type: "text", nullable: false),
                Query = table.Column<string>(type: "text", nullable: false),
                Response = table.Column<string>(type: "text", nullable: false),
                Publication_Authors = table.Column<string[]>(type: "text[]", nullable: true),
                Publication_Doi = table.Column<string>(type: "text", nullable: true),
                Publication_ArXiv = table.Column<string>(type: "text", nullable: true),
                Publication_Urn = table.Column<string>(type: "text", nullable: true),
                Publication_WebAddress = table.Column<string>(type: "text", nullable: true),
                Publication_Title = table.Column<string>(type: "text", nullable: true),
                Publication_Abstract = table.Column<string>(type: "text", nullable: true),
                Publication_Section = table.Column<string>(type: "text", nullable: true),
                Standard_Year = table.Column<int>(type: "integer", nullable: true),
                Standard_Numeration_Prefix = table.Column<string>(type: "text", nullable: true),
                Standard_Numeration_MainNumber = table.Column<string>(type: "text", nullable: true),
                Standard_Numeration_Suffix = table.Column<string>(type: "text", nullable: true),
                Standard_Standardizers = table.Column<Standardizer[]>(type: "database.standardizer[]", nullable: true),
                Standard_Locator = table.Column<string>(type: "text", nullable: true),
                Standard_Title = table.Column<string>(type: "text", nullable: true),
                Standard_Abstract = table.Column<string>(type: "text", nullable: true),
                Standard_Section = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_geometric_data_Approvals", x => new { x.GeometricDataId, x.Id });
                table.ForeignKey(
                    name: "FK_geometric_data_Approvals_geometric_data_GeometricDataId",
                    column: x => x.GeometricDataId,
                    principalSchema: "database",
                    principalTable: "geometric_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "geometric_data_Arguments",
            schema: "database",
            columns: table => new
            {
                AppliedMethodGeometricDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<JsonDocument>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_geometric_data_Arguments", x => new { x.AppliedMethodGeometricDataId, x.Id });
                table.ForeignKey(
                    name: "FK_geometric_data_Arguments_geometric_data_AppliedMethodGeomet~",
                    column: x => x.AppliedMethodGeometricDataId,
                    principalSchema: "database",
                    principalTable: "geometric_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "geometric_data_Sources",
            schema: "database",
            columns: table => new
            {
                AppliedMethodGeometricDataId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Value_DataId = table.Column<Guid>(type: "uuid", nullable: false),
                Value_DataTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Value_DataKind = table.Column<DataKind>(type: "database.data_kind", nullable: false),
                Value_DatabaseId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_geometric_data_Sources", x => new { x.AppliedMethodGeometricDataId, x.Id });
                table.ForeignKey(
                    name: "FK_geometric_data_Sources_geometric_data_AppliedMethodGeometri~",
                    column: x => x.AppliedMethodGeometricDataId,
                    principalSchema: "database",
                    principalTable: "geometric_data",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_get_https_resource_GeometricDataId",
            schema: "database",
            table: "get_https_resource",
            column: "GeometricDataId");

        migrationBuilder.AddForeignKey(
            name: "FK_get_https_resource_geometric_data_GeometricDataId",
            schema: "database",
            table: "get_https_resource",
            column: "GeometricDataId",
            principalSchema: "database",
            principalTable: "geometric_data",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_get_https_resource_geometric_data_GeometricDataId",
            schema: "database",
            table: "get_https_resource");

        migrationBuilder.DropTable(
            name: "geometric_data_Approvals",
            schema: "database");

        migrationBuilder.DropTable(
            name: "geometric_data_Arguments",
            schema: "database");

        migrationBuilder.DropTable(
            name: "geometric_data_Sources",
            schema: "database");

        migrationBuilder.DropTable(
            name: "geometric_data",
            schema: "database");

        migrationBuilder.DropIndex(
            name: "IX_get_https_resource_GeometricDataId",
            schema: "database",
            table: "get_https_resource");

        migrationBuilder.DropColumn(
            name: "GeometricDataId",
            schema: "database",
            table: "get_https_resource");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:database.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .OldAnnotation("Npgsql:Enum:database.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data,geometric_data");
    }
}
