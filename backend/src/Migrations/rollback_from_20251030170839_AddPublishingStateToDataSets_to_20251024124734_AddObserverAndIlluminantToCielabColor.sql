START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.photovoltaic_data DROP COLUMN "PublishingState";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.optical_data DROP COLUMN "PublishingState";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.hygrothermal_data DROP COLUMN "PublishingState";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "PublishingState";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.calorimetric_data DROP COLUMN "PublishingState";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    DROP TYPE database.publishing_state;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets';
    END IF;
END $EF$;
COMMIT;

