START TRANSACTION;
DROP INDEX database."IX_institution_access_rights_InstitutionId";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250731163636_MakeInstitutionIdOfAccessRightsUnique';

ALTER TABLE database."OpenIddictTokens" ALTER COLUMN "Type" TYPE character varying(50);

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250725172127_UpgradeOpenIddictToVersion7';

COMMIT;

