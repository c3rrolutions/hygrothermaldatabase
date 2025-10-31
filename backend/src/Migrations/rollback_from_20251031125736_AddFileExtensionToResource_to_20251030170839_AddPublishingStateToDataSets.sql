START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251031125736_AddFileExtensionToResource') THEN
    ALTER TABLE database.get_https_resource DROP COLUMN "FileExtension";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251031125736_AddFileExtensionToResource') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20251031125736_AddFileExtensionToResource';
    END IF;
END $EF$;
COMMIT;

