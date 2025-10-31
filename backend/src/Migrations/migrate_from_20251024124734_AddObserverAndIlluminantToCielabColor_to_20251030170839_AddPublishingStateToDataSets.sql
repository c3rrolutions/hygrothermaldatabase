START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    CREATE TYPE database.publishing_state AS ENUM ('pending', 'published', 'retracted');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.photovoltaic_data ADD "PublishingState" database.publishing_state NOT NULL DEFAULT 'pending'::database.publishing_state;
    UPDATE database.photovoltaic_data SET "PublishingState" = 'published';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.optical_data ADD "PublishingState" database.publishing_state NOT NULL DEFAULT 'pending'::database.publishing_state;
    UPDATE database.optical_data SET "PublishingState" = 'published';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.hygrothermal_data ADD "PublishingState" database.publishing_state NOT NULL DEFAULT 'pending'::database.publishing_state;
    UPDATE database.hygrothermal_data SET "PublishingState" = 'published';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.geometric_data ADD "PublishingState" database.publishing_state NOT NULL DEFAULT 'pending'::database.publishing_state;
    UPDATE database.geometric_data SET "PublishingState" = 'published';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    ALTER TABLE database.calorimetric_data ADD "PublishingState" database.publishing_state NOT NULL DEFAULT 'pending'::database.publishing_state;
    UPDATE database.calorimetric_data SET "PublishingState" = 'published';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251030170839_AddPublishingStateToDataSets') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251030170839_AddPublishingStateToDataSets', '9.0.10');
    END IF;
END $EF$;
COMMIT;

