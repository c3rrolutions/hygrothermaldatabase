START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023152609_RememberUserWhoCreatedDataSet') THEN
    ALTER TABLE database.photovoltaic_data DROP COLUMN "UserId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023152609_RememberUserWhoCreatedDataSet') THEN
    ALTER TABLE database.optical_data DROP COLUMN "UserId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023152609_RememberUserWhoCreatedDataSet') THEN
    ALTER TABLE database.hygrothermal_data DROP COLUMN "UserId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023152609_RememberUserWhoCreatedDataSet') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "UserId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023152609_RememberUserWhoCreatedDataSet') THEN
    ALTER TABLE database.calorimetric_data DROP COLUMN "UserId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251023152609_RememberUserWhoCreatedDataSet') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20251023152609_RememberUserWhoCreatedDataSet';
    END IF;
END $EF$;
COMMIT;

