START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    DROP TABLE database.institution_access_policy;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    DROP TABLE database.open_id_connect_application_access_policy;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    DROP TABLE database.user_access_policy;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data RENAME COLUMN "AccessPolicy" TO "DataAccessRights_AllowedUserAndQuantity";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.optical_data RENAME COLUMN "AccessPolicy" TO "DataAccessRights_AllowedUserAndQuantity";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" RENAME COLUMN "AccessPolicy" TO "DataAccessRights_AllowedUserAndQuantity";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data RENAME COLUMN "AccessPolicy" TO "DataAccessRights_AllowedUserAndQuantity";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.geometric_data RENAME COLUMN "AccessPolicy" TO "DataAccessRights_AllowedUserAndQuantity";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data RENAME COLUMN "AccessPolicy" TO "DataAccessRights_AllowedUserAndQuantity";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    DROP TYPE database.logical_combinator;
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
            CREATE SCHEMA database;
        END IF;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE EXTENSION IF NOT EXISTS pgcrypto;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data ADD "DataAccessRights_AllowedApplications" text[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data ADD "DataAccessRights_AllowedInstitutions" uuid[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.optical_data ADD "DataAccessRights_AllowedApplications" text[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.optical_data ADD "DataAccessRights_AllowedInstitutions" uuid[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" ADD "DataAccessRights_AllowedApplications" text[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" ADD "DataAccessRights_AllowedInstitutions" uuid[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data ADD "DataAccessRights_AllowedApplications" text[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data ADD "DataAccessRights_AllowedInstitutions" uuid[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.geometric_data ADD "DataAccessRights_AllowedApplications" text[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.geometric_data ADD "DataAccessRights_AllowedInstitutions" uuid[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data ADD "DataAccessRights_AllowedApplications" text[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data ADD "DataAccessRights_AllowedInstitutions" uuid[];
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE TABLE database.institution_access_rights (
        "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
        "AllowedDatasetsPerTime" bigint,
        "AllowedUserCount" bigint,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now()),
        "InstitutionId" uuid NOT NULL,
        "Period" interval NOT NULL,
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now()),
        "UserAlreadyAccessed" uuid[] NOT NULL,
        CONSTRAINT "PK_institution_access_rights" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_institution_access_rights_CreatedAt_Id" ON database.institution_access_rights ("CreatedAt", "Id");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_institution_access_rights_InstitutionId" ON database.institution_access_rights ("InstitutionId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260603191121_RevampAccessPolicies') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260603191121_RevampAccessPolicies';
    END IF;
END $EF$;
COMMIT;

