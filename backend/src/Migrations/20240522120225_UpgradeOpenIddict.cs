using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class UpgradeOpenIddict : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Email",
            schema: "database",
            table: "user");

        migrationBuilder.RenameColumn(
            name: "Type",
            schema: "database",
            table: "OpenIddictApplications",
            newName: "ClientType");

        migrationBuilder.AddColumn<string>(
            name: "ApplicationType",
            schema: "database",
            table: "OpenIddictApplications",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "JsonWebKeySet",
            schema: "database",
            table: "OpenIddictApplications",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Settings",
            schema: "database",
            table: "OpenIddictApplications",
            type: "text",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ApplicationType",
            schema: "database",
            table: "OpenIddictApplications");

        migrationBuilder.DropColumn(
            name: "JsonWebKeySet",
            schema: "database",
            table: "OpenIddictApplications");

        migrationBuilder.DropColumn(
            name: "Settings",
            schema: "database",
            table: "OpenIddictApplications");

        migrationBuilder.RenameColumn(
            name: "ClientType",
            schema: "database",
            table: "OpenIddictApplications",
            newName: "Type");

        migrationBuilder.AddColumn<string>(
            name: "Email",
            schema: "database",
            table: "user",
            type: "text",
            nullable: false,
            defaultValue: "");
    }
}
