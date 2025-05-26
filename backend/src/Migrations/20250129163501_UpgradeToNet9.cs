using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeToNet9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.data_kind", "calorimetric_data,geometric_data,hygrothermal_data,optical_data,photovoltaic_data")
                .OldAnnotation("Npgsql:Enum:database.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data,geometric_data");

            migrationBuilder.AlterColumn<DataKind>(
                name: "Value_DataKind",
                schema: "database",
                table: "geometric_data_Sources",
                type: "database.data_kind",
                nullable: false,
                oldClrType: typeof(DataKind),
                oldType: "data_kind");

            migrationBuilder.AlterColumn<Standardizer[]>(
                name: "Standard_Standardizers",
                schema: "database",
                table: "geometric_data_Approvals",
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
                .Annotation("Npgsql:Enum:database.data_kind", "calorimetric_data,hygrothermal_data,optical_data,photovoltaic_data,geometric_data")
                .OldAnnotation("Npgsql:Enum:database.data_kind", "calorimetric_data,geometric_data,hygrothermal_data,optical_data,photovoltaic_data");

            migrationBuilder.AlterColumn<DataKind>(
                name: "Value_DataKind",
                schema: "database",
                table: "geometric_data_Sources",
                type: "data_kind",
                nullable: false,
                oldClrType: typeof(DataKind),
                oldType: "database.data_kind");

            migrationBuilder.AlterColumn<Standardizer[]>(
                name: "Standard_Standardizers",
                schema: "database",
                table: "geometric_data_Approvals",
                type: "standardizer[]",
                nullable: true,
                oldClrType: typeof(Standardizer[]),
                oldType: "database.standardizer[]",
                oldNullable: true);
        }
    }
}