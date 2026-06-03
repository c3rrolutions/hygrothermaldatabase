START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    DROP TABLE database.institution_access_rights;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data DROP COLUMN "DataAccessRights_AllowedApplications";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data DROP COLUMN "DataAccessRights_AllowedInstitutions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.optical_data DROP COLUMN "DataAccessRights_AllowedApplications";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.optical_data DROP COLUMN "DataAccessRights_AllowedInstitutions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" DROP COLUMN "DataAccessRights_AllowedApplications";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" DROP COLUMN "DataAccessRights_AllowedInstitutions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data DROP COLUMN "DataAccessRights_AllowedApplications";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data DROP COLUMN "DataAccessRights_AllowedInstitutions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "DataAccessRights_AllowedApplications";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "DataAccessRights_AllowedInstitutions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data DROP COLUMN "DataAccessRights_AllowedApplications";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data DROP COLUMN "DataAccessRights_AllowedInstitutions";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";
    ALTER TABLE database.photovoltaic_data ADD COLUMN "AccessPolicy" jsonb;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.optical_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";
    ALTER TABLE database.optical_data ADD COLUMN "AccessPolicy" jsonb;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";
    ALTER TABLE database."lifeCycle_data" ADD COLUMN "AccessPolicy" jsonb;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";
    ALTER TABLE database.hygrothermal_data ADD COLUMN "AccessPolicy" jsonb;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";
    ALTER TABLE database.geometric_data ADD COLUMN "AccessPolicy" jsonb;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";
    ALTER TABLE database.calorimetric_data ADD COLUMN "AccessPolicy" jsonb;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
            CREATE SCHEMA database;
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE TYPE database.logical_combinator AS ENUM ('all', 'some');
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
            CREATE SCHEMA database;
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE EXTENSION IF NOT EXISTS pgcrypto;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE TABLE database.institution_access_policy (
        "InstitutionId" uuid NOT NULL,
        "AccessCountSinceStartTime" jsonb,
        "UpperAccessLimitPerTimeDuration" jsonb,
        CONSTRAINT "PK_institution_access_policy" PRIMARY KEY ("InstitutionId")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE TABLE database.open_id_connect_application_access_policy (
        "ClientId" text NOT NULL,
        "AccessCountSinceStartTime" jsonb,
        "UpperAccessLimitPerTimeDuration" jsonb,
        CONSTRAINT "PK_open_id_connect_application_access_policy" PRIMARY KEY ("ClientId")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE TABLE database.user_access_policy (
        "UserId" uuid NOT NULL,
        "AccessCountSinceStartTime" jsonb,
        "UpperAccessLimitPerTimeDuration" jsonb,
        CONSTRAINT "PK_user_access_policy" PRIMARY KEY ("UserId")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260603191121_RevampAccessPolicies', '10.0.8');
    END IF;
END $EF$;
COMMIT;

