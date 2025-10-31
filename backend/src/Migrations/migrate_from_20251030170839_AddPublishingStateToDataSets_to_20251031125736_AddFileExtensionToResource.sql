START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251031125736_AddFileExtensionToResource') THEN
    ALTER TABLE database.get_https_resource ADD "FileExtension" text;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251031125736_AddFileExtensionToResource') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251031125736_AddFileExtensionToResource', '9.0.10');
    END IF;
END $EF$;
COMMIT;

