using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class MoveInstitutionAccessRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_access_rights",
                schema: "database",
                table: "access_rights");

            migrationBuilder.RenameTable(
                name: "access_rights",
                schema: "database",
                newName: "institution_access_rights",
                newSchema: "database");

            migrationBuilder.AddPrimaryKey(
                name: "PK_institution_access_rights",
                schema: "database",
                table: "institution_access_rights",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_institution_access_rights",
                schema: "database",
                table: "institution_access_rights");

            migrationBuilder.RenameTable(
                name: "institution_access_rights",
                schema: "database",
                newName: "access_rights",
                newSchema: "database");

            migrationBuilder.AddPrimaryKey(
                name: "PK_access_rights",
                schema: "database",
                table: "access_rights",
                column: "Id");
        }
    }
}
