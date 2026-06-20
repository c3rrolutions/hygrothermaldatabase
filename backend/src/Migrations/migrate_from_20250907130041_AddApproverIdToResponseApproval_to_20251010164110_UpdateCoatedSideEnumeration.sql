START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010164110_UpdateCoatedSideEnumeration') THEN

        ALTER TYPE database.coated_side RENAME TO coated_side_old;
        CREATE TYPE database.coated_side AS ENUM ('both', 'neither', 'non_prime', 'not_applicable', 'prime', 'unknown');
        ALTER TABLE database.optical_data ALTER COLUMN "CoatedSide" TYPE database.coated_side USING "CoatedSide"::text::database.coated_side;
        DROP TYPE database.coated_side_old;

        INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
        VALUES ('20251010164110_UpdateCoatedSideEnumeration', '9.0.8');

    END IF;
END $EF$;

COMMIT;
