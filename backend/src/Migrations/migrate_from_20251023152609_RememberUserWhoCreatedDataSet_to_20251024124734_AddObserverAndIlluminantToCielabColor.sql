START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
            CREATE SCHEMA database;
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    CREATE TYPE database.calorimetric_observer AS ENUM ('ten_degrees', 'two_degrees');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    CREATE TYPE database.illuminant AS ENUM ('a', 'd65');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."optical_data_Approvals" ALTER COLUMN "Variables" DROP DEFAULT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "Approval_Variables" DROP DEFAULT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."CielabColor" ADD "Illuminant" database.illuminant;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."CielabColor" ADD "Observer" database.calorimetric_observer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."CielabColor" ADD CONSTRAINT "CK_OpticalData_CielabColors_AStar" CHECK ("AStar" >= 0.0 AND "AStar" <= 100.0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."CielabColor" ADD CONSTRAINT "CK_OpticalData_CielabColors_BStar" CHECK ("BStar" >= 0.0 AND "BStar" <= 100.0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    ALTER TABLE database."CielabColor" ADD CONSTRAINT "CK_OpticalData_CielabColors_LStar" CHECK ("LStar" >= 0.0 AND "LStar" <= 100.0);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251024124734_AddObserverAndIlluminantToCielabColor') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251024124734_AddObserverAndIlluminantToCielabColor', '9.0.10');
    END IF;
END $EF$;
COMMIT;

