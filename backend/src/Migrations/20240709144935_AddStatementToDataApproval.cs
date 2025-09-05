using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class AddStatementToDataApproval : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:public.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .Annotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
            .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
            .OldAnnotation("Npgsql:Enum:public.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");

        migrationBuilder.AddColumn<string>(
            name: "Publication_Abstract",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_ArXiv",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string[]>(
            name: "Publication_Authors",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text[]",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Doi",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Section",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Title",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Urn",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_WebAddress",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Abstract",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Locator",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Section",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "standardizer[]",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Title",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Standard_Year",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Abstract",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_ArXiv",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string[]>(
            name: "Publication_Authors",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text[]",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Doi",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Section",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Title",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Urn",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_WebAddress",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Abstract",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Locator",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Section",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "optical_data_Approvals",
            type: "standardizer[]",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Title",
            schema: "database",
            table: "optical_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Standard_Year",
            schema: "database",
            table: "optical_data_Approvals",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Abstract",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_ArXiv",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string[]>(
            name: "Publication_Authors",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text[]",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Doi",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Section",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Title",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Urn",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_WebAddress",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Abstract",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Locator",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Section",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "standardizer[]",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Title",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Standard_Year",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Abstract",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_ArXiv",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string[]>(
            name: "Publication_Authors",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text[]",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Doi",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Section",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Title",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_Urn",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Publication_WebAddress",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Abstract",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Locator",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Section",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<Standardizer[]>(
            name: "Standard_Standardizers",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "standardizer[]",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Standard_Title",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Standard_Year",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "integer",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Section",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Title",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Section",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Title",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Year",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Section",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Title",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Section",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Title",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Year",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Section",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Title",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Section",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Title",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Year",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Section",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Title",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Section",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Title",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Standard_Year",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:public.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
            .OldAnnotation("Npgsql:Enum:public.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data")
            .OldAnnotation("Npgsql:Enum:public.standardizer", "aerc,agi,ashrae,breeam,bs,bsi,cen,cie,dgnb,din,dvwg,iec,ies,ift,iso,jis,leed,nfrc,riba,ul,unece,vdi,vff,well")
            .OldAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,");
    }
}
