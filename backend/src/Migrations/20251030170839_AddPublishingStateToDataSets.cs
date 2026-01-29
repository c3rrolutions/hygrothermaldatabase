using Database.Enumerations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishingStateToDataSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.publishing_state", "pending,published,retracted");

            migrationBuilder.AddColumn<PublishingState>(
                name: "PublishingState",
                schema: "database",
                table: "photovoltaic_data",
                type: "database.publishing_state",
                nullable: false,
                defaultValue: PublishingState.PENDING);

            migrationBuilder.AddColumn<PublishingState>(
                name: "PublishingState",
                schema: "database",
                table: "optical_data",
                type: "database.publishing_state",
                nullable: false,
                defaultValue: PublishingState.PENDING);

            migrationBuilder.AddColumn<PublishingState>(
                name: "PublishingState",
                schema: "database",
                table: "hygrothermal_data",
                type: "database.publishing_state",
                nullable: false,
                defaultValue: PublishingState.PENDING);

            migrationBuilder.AddColumn<PublishingState>(
                name: "PublishingState",
                schema: "database",
                table: "geometric_data",
                type: "database.publishing_state",
                nullable: false,
                defaultValue: PublishingState.PENDING);

            migrationBuilder.AddColumn<PublishingState>(
                name: "PublishingState",
                schema: "database",
                table: "calorimetric_data",
                type: "database.publishing_state",
                nullable: false,
                defaultValue: PublishingState.PENDING);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishingState",
                schema: "database",
                table: "photovoltaic_data");

            migrationBuilder.DropColumn(
                name: "PublishingState",
                schema: "database",
                table: "optical_data");

            migrationBuilder.DropColumn(
                name: "PublishingState",
                schema: "database",
                table: "hygrothermal_data");

            migrationBuilder.DropColumn(
                name: "PublishingState",
                schema: "database",
                table: "geometric_data");

            migrationBuilder.DropColumn(
                name: "PublishingState",
                schema: "database",
                table: "calorimetric_data");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:database.publishing_state", "pending,published,retracted");
        }
    }
}