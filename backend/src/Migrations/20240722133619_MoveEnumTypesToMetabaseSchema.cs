using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class MoveEnumTypesToMetabaseSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:database.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .Annotation("Npgsql:Enum:database.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
            .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
            .OldAnnotation("Npgsql:Enum:public.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .OldAnnotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
            .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");

        migrationBuilder.AlterColumn<DataKind>(
            name: "Value_DataKind",
            schema: "database",
            table: "photovoltaic_data_Sources",
            type: "database.data_kind",
            nullable: false,
            oldClrType: typeof(DataKind),
            oldType: "data_kind");

        migrationBuilder.AlterColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "database.standardizer[]",
            nullable: true,
            oldClrType: typeof(Standardizer[]),
            oldType: "standardizer[]",
            oldNullable: true);

        migrationBuilder.AlterColumn<DataKind>(
            name: "Value_DataKind",
            schema: "database",
            table: "optical_data_Sources",
            type: "database.data_kind",
            nullable: false,
            oldClrType: typeof(DataKind),
            oldType: "data_kind");

        migrationBuilder.AlterColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "optical_data_Approvals",
            type: "database.standardizer[]",
            nullable: true,
            oldClrType: typeof(Standardizer[]),
            oldType: "standardizer[]",
            oldNullable: true);

        migrationBuilder.AlterColumn<DataKind>(
            name: "Value_DataKind",
            schema: "database",
            table: "hygrothermal_data_Sources",
            type: "database.data_kind",
            nullable: false,
            oldClrType: typeof(DataKind),
            oldType: "data_kind");

        migrationBuilder.AlterColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "database.standardizer[]",
            nullable: true,
            oldClrType: typeof(Standardizer[]),
            oldType: "standardizer[]",
            oldNullable: true);

        migrationBuilder.AlterColumn<DataKind>(
            name: "Value_DataKind",
            schema: "database",
            table: "calorimetric_data_Sources",
            type: "database.data_kind",
            nullable: false,
            oldClrType: typeof(DataKind),
            oldType: "data_kind");

        migrationBuilder.AlterColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "database.standardizer[]",
            nullable: true,
            oldClrType: typeof(Standardizer[]),
            oldType: "standardizer[]",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:public.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .Annotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
            .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
            .OldAnnotation("Npgsql:Enum:database.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .OldAnnotation("Npgsql:Enum:database.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
            .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");

        migrationBuilder.AlterColumn<DataKind>(
            name: "Value_DataKind",
            schema: "database",
            table: "photovoltaic_data_Sources",
            type: "data_kind",
            nullable: false,
            oldClrType: typeof(DataKind),
            oldType: "database.data_kind");

        migrationBuilder.AlterColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "standardizer[]",
            nullable: true,
            oldClrType: typeof(Standardizer[]),
            oldType: "database.standardizer[]",
            oldNullable: true);

        migrationBuilder.AlterColumn<DataKind>(
            name: "Value_DataKind",
            schema: "database",
            table: "optical_data_Sources",
            type: "data_kind",
            nullable: false,
            oldClrType: typeof(DataKind),
            oldType: "database.data_kind");

        migrationBuilder.AlterColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "optical_data_Approvals",
            type: "standardizer[]",
            nullable: true,
            oldClrType: typeof(Standardizer[]),
            oldType: "database.standardizer[]",
            oldNullable: true);

        migrationBuilder.AlterColumn<DataKind>(
            name: "Value_DataKind",
            schema: "database",
            table: "hygrothermal_data_Sources",
            type: "data_kind",
            nullable: false,
            oldClrType: typeof(DataKind),
            oldType: "database.data_kind");

        migrationBuilder.AlterColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "standardizer[]",
            nullable: true,
            oldClrType: typeof(Standardizer[]),
            oldType: "database.standardizer[]",
            oldNullable: true);

        migrationBuilder.AlterColumn<DataKind>(
            name: "Value_DataKind",
            schema: "database",
            table: "calorimetric_data_Sources",
            type: "data_kind",
            nullable: false,
            oldClrType: typeof(DataKind),
            oldType: "database.data_kind");

        migrationBuilder.AlterColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "standardizer[]",
            nullable: true,
            oldClrType: typeof(Standardizer[]),
            oldType: "database.standardizer[]",
            oldNullable: true);
    }
}
