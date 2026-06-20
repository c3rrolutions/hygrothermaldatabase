START TRANSACTION;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260620114038_MakeCoatedSideOfOpticalDataNonNullable') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "CoatedSide" DROP NOT NULL;
    END IF;
END $EF$;

COMMIT;
