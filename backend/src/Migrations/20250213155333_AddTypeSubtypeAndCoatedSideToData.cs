using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeSubtypeAndCoatedSideToData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.coated_side", "back,both,front,neither")
                .Annotation("Npgsql:Enum:database.optical_component_subtype", "acid_etched_glass,applied_film,cellular_shade,chromogenic,coated,coating,diffusing_shade,embedded_coating,film,fritted_glass,interlayer,laminate,monolithic,perforated_screen,pleated_shade,roller_shade,roman_shade,sandblasted_glass,shade_material,venetian_blind,vertical_louver,woven_shade")
                .Annotation("Npgsql:Enum:database.optical_component_type", "glazing,shading");

            migrationBuilder.AddColumn<CoatedSide>(
                name: "CoatedSide",
                schema: "database",
                table: "optical_data",
                type: "database.coated_side",
                nullable: true);

            migrationBuilder.AddColumn<OpticalComponentSubtype>(
                name: "Subtype",
                schema: "database",
                table: "optical_data",
                type: "database.optical_component_subtype",
                nullable: true);

            migrationBuilder.AddColumn<OpticalComponentType>(
                name: "Type",
                schema: "database",
                table: "optical_data",
                type: "database.optical_component_type",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoatedSide",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "Subtype",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "database",
                table: "optical_data");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:database.coated_side", "back,both,front,neither")
                .OldAnnotation("Npgsql:Enum:database.optical_component_subtype", "acid_etched_glass,applied_film,cellular_shade,chromogenic,coated,coating,diffusing_shade,embedded_coating,film,fritted_glass,interlayer,laminate,monolithic,perforated_screen,pleated_shade,roller_shade,roman_shade,sandblasted_glass,shade_material,venetian_blind,vertical_louver,woven_shade")
                .OldAnnotation("Npgsql:Enum:database.optical_component_type", "glazing,shading");
        }
    }
}