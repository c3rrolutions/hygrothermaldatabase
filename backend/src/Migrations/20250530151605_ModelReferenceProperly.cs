using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class ModelReferenceProperly : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Standard_Year",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Standard_Title",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Standard_Section",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Publication_Title",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Publication_Section",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Statement_Publication_Abstract");

        migrationBuilder.RenameColumn(
            name: "Standard_Year",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Standard_Title",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Standard_Section",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Publication_Title",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Publication_Section",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Statement_Publication_Abstract");

        migrationBuilder.RenameColumn(
            name: "Standard_Year",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Standard_Title",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Standard_Section",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Publication_Title",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Publication_Section",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Statement_Publication_Abstract");

        migrationBuilder.RenameColumn(
            name: "Standard_Year",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Standard_Title",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Standard_Section",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Publication_Title",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Publication_Section",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Statement_Publication_Abstract");

        migrationBuilder.RenameColumn(
            name: "Standard_Year",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Standard_Title",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Standard_Standardizers",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Standard_Section",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Suffix",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_Prefix",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Standard_Numeration_MainNumber",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Standard_Locator",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Standard_Abstract",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Publication_WebAddress",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Publication_Urn",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Publication_Title",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Publication_Section",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Publication_Doi",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Publication_Authors",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Publication_ArXiv",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Publication_Abstract",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Statement_Publication_Abstract");

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Exists",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Exists",
            schema: "database",
            table: "optical_data_Approvals",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Exists",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Exists",
            schema: "database",
            table: "geometric_data_Approvals",
            type: "boolean",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<bool>(
            name: "Statement_Exists",
            schema: "database",
            table: "calorimetric_data_Approvals",
            type: "boolean",
            nullable: false,
            defaultValue: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Statement_Exists",
            schema: "database",
            table: "photovoltaic_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Statement_Exists",
            schema: "database",
            table: "optical_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Statement_Exists",
            schema: "database",
            table: "hygrothermal_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Statement_Exists",
            schema: "database",
            table: "geometric_data_Approvals");

        migrationBuilder.DropColumn(
            name: "Statement_Exists",
            schema: "database",
            table: "calorimetric_data_Approvals");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Year",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Title",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Standardizers",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Section",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Suffix",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Prefix",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_MainNumber",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Locator",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Abstract",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_WebAddress",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Urn",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Title",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Section",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Doi",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Authors",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_ArXiv",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Abstract",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Publication_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Year",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Title",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Standardizers",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Section",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Suffix",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Prefix",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_MainNumber",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Locator",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Abstract",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_WebAddress",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Urn",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Title",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Section",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Doi",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Authors",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_ArXiv",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Abstract",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Publication_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Year",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Title",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Standardizers",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Section",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Suffix",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Prefix",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_MainNumber",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Locator",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Abstract",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_WebAddress",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Urn",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Title",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Section",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Doi",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Authors",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_ArXiv",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Abstract",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Publication_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Year",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Title",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Standardizers",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Section",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Suffix",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Prefix",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_MainNumber",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Locator",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Abstract",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_WebAddress",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Urn",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Title",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Section",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Doi",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Authors",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_ArXiv",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Abstract",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Publication_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Year",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Year");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Title",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Standardizers",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Standardizers");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Section",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Suffix",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Numeration_Suffix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_Prefix",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Numeration_Prefix");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Numeration_MainNumber",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Numeration_MainNumber");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Locator",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Locator");

        migrationBuilder.RenameColumn(
            name: "Statement_Standard_Abstract",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Standard_Abstract");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_WebAddress",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Publication_WebAddress");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Urn",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Publication_Urn");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Title",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Publication_Title");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Section",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Publication_Section");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Doi",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Publication_Doi");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Authors",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Publication_Authors");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_ArXiv",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Publication_ArXiv");

        migrationBuilder.RenameColumn(
            name: "Statement_Publication_Abstract",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Publication_Abstract");
    }
}