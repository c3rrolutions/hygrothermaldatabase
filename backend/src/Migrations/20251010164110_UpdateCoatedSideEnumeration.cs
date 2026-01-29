using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCoatedSideEnumeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.coated_side", "both,neither,non_prime,not_applicable,prime,unknown")
                .OldAnnotation("Npgsql:Enum:database.coated_side", "back,both,front,neither");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:database.coated_side", "back,both,front,neither")
                .OldAnnotation("Npgsql:Enum:database.coated_side", "both,neither,non_prime,not_applicable,prime,unknown");
        }
    }
}