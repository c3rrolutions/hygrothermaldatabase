START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260620114038_MakeCoatedSideOfOpticalDataNonNullable') THEN
    UPDATE database.optical_data SET "CoatedSide" = 'unknown'::database.coated_side WHERE "CoatedSide" IS NULL;
    ALTER TABLE database.optical_data ALTER COLUMN "CoatedSide" SET NOT NULL;
    ALTER TABLE database.optical_data ALTER COLUMN "CoatedSide" SET DEFAULT 'unknown'::database.coated_side;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260620114038_MakeCoatedSideOfOpticalDataNonNullable') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260620114038_MakeCoatedSideOfOpticalDataNonNullable', '10.0.9');
    END IF;
END $EF$;
COMMIT;

