START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."CielabColor" DROP CONSTRAINT "CK_OpticalData_CielabColors_LStar";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."CielabColor" DROP COLUMN "Illuminant";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."CielabColor" DROP COLUMN "Observer";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    DROP TYPE database.calorimetric_observer;
    DROP TYPE database.illuminant;
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
            CREATE SCHEMA database;
        END IF;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."optical_data_Approvals" ALTER COLUMN "Variables" SET DEFAULT ('{}');
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "Approval_Variables" SET DEFAULT ('{}');
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor';
    END IF;
END $EF$;
COMMIT;

