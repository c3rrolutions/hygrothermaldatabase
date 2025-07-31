using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeInstitutionIdOfAccessRightsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_institution_access_rights_InstitutionId",
                schema: "database",
                table: "institution_access_rights",
                column: "InstitutionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_institution_access_rights_InstitutionId",
                schema: "database",
                table: "institution_access_rights");
        }
    }
}
