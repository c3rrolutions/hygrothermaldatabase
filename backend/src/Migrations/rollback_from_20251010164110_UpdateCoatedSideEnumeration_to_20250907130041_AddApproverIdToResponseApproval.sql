START TRANSACTION;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010164110_UpdateCoatedSideEnumeration') THEN

        ALTER TYPE database.coated_side RENAME TO coated_side_old;
        CREATE TYPE database.coated_side AS ENUM ('back', 'both', 'front', 'neither');
        ALTER TABLE database.optical_data ALTER COLUMN "CoatedSide" TYPE database.coated_side USING "CoatedSide"::text::database.coated_side;
        DROP TYPE database.coated_side_old;

        DELETE FROM "__EFMigrationsHistory"
        WHERE "MigrationId" = '20251010164110_UpdateCoatedSideEnumeration';

    END IF;
END $EF$;

COMMIT;
