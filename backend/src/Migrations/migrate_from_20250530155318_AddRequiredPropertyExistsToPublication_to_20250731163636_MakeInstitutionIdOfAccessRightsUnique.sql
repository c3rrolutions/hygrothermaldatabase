START TRANSACTION;
ALTER TABLE database."OpenIddictTokens" ALTER COLUMN "Type" TYPE character varying(150);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250725172127_UpgradeOpenIddictToVersion7', '9.0.7');

CREATE UNIQUE INDEX "IX_institution_access_rights_InstitutionId" ON database.institution_access_rights ("InstitutionId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250731163636_MakeInstitutionIdOfAccessRightsUnique', '9.0.7');

COMMIT;

