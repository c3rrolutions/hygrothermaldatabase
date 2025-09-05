using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class OpenIddict : Migration
{
    private static readonly string[] _applicationIdStatusSubjectAndTypeIndexColumns = ["ApplicationId", "Status", "Subject", "Type"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "OpenIddictApplications",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                ClientId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ClientSecret = table.Column<string>(type: "text", nullable: true),
                ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                ConsentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                DisplayName = table.Column<string>(type: "text", nullable: true),
                DisplayNames = table.Column<string>(type: "text", nullable: true),
                Permissions = table.Column<string>(type: "text", nullable: true),
                PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                RedirectUris = table.Column<string>(type: "text", nullable: true),
                Requirements = table.Column<string>(type: "text", nullable: true),
                Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OpenIddictScopes",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Description = table.Column<string>(type: "text", nullable: true),
                Descriptions = table.Column<string>(type: "text", nullable: true),
                DisplayName = table.Column<string>(type: "text", nullable: true),
                DisplayNames = table.Column<string>(type: "text", nullable: true),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                Resources = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OpenIddictAuthorizations",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                ApplicationId = table.Column<string>(type: "text", nullable: true),
                ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                Scopes = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
                table.ForeignKey(
                    name: "FK_OpenIddictAuthorizations_OpenIddictApplications_Application~",
                    column: x => x.ApplicationId,
                    principalSchema: "database",
                    principalTable: "OpenIddictApplications",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "OpenIddictTokens",
            schema: "database",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                ApplicationId = table.Column<string>(type: "text", nullable: true),
                AuthorizationId = table.Column<string>(type: "text", nullable: true),
                ConcurrencyToken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Payload = table.Column<string>(type: "text", nullable: true),
                Properties = table.Column<string>(type: "text", nullable: true),
                RedemptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                Subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
                table.ForeignKey(
                    name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                    column: x => x.ApplicationId,
                    principalSchema: "database",
                    principalTable: "OpenIddictApplications",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                    column: x => x.AuthorizationId,
                    principalSchema: "database",
                    principalTable: "OpenIddictAuthorizations",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictApplications_ClientId",
            schema: "database",
            table: "OpenIddictApplications",
            column: "ClientId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
            schema: "database",
            table: "OpenIddictAuthorizations",
            columns: _applicationIdStatusSubjectAndTypeIndexColumns);

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictScopes_Name",
            schema: "database",
            table: "OpenIddictScopes",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
            schema: "database",
            table: "OpenIddictTokens",
            columns: _applicationIdStatusSubjectAndTypeIndexColumns);

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_AuthorizationId",
            schema: "database",
            table: "OpenIddictTokens",
            column: "AuthorizationId");

        migrationBuilder.CreateIndex(
            name: "IX_OpenIddictTokens_ReferenceId",
            schema: "database",
            table: "OpenIddictTokens",
            column: "ReferenceId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OpenIddictScopes",
            schema: "database");

        migrationBuilder.DropTable(
            name: "OpenIddictTokens",
            schema: "database");

        migrationBuilder.DropTable(
            name: "OpenIddictAuthorizations",
            schema: "database");

        migrationBuilder.DropTable(
            name: "OpenIddictApplications",
            schema: "database");
    }
}
