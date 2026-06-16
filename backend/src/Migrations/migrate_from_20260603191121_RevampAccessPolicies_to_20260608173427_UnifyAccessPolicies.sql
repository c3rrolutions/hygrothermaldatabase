START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_id_cannot_change"() CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_ids_must_match"() CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP CONSTRAINT "PK_user_access_policy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP CONSTRAINT "PK_open_id_connect_application_access_policy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP CONSTRAINT "PK_institution_access_policy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "AccessCountSinceStartTime";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data DROP COLUMN "AccessPolicy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.optical_data DROP COLUMN "AccessPolicy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "AccessCountSinceStartTime";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" DROP COLUMN "AccessPolicy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "AccessCountSinceStartTime";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data DROP COLUMN "AccessPolicy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "AccessPolicy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data DROP COLUMN "AccessPolicy";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "Id" uuid NOT NULL DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "AccessCountSinceStartTime_AccessCount" bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "AccessCountSinceStartTime_StartTime" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "DataAccessPolicyId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "UpperAccessLimitPerTimeDuration_Duration" interval;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "UpperAccessLimitPerTimeDuration_UpperLimit" bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database."user" ALTER COLUMN "Id" SET DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data ALTER COLUMN "Id" SET DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "Id" SET DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "Id" uuid NOT NULL DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "AccessCountSinceStartTime_AccessCount" bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "AccessCountSinceStartTime_StartTime" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "DataAccessPolicyId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "UpperAccessLimitPerTimeDuration_Duration" interval;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "UpperAccessLimitPerTimeDuration_UpperLimit" bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" ALTER COLUMN "Id" SET DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "Id" uuid NOT NULL DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "AccessCountSinceStartTime_AccessCount" bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "AccessCountSinceStartTime_StartTime" timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "DataAccessPolicyId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "UpperAccessLimitPerTimeDuration_Duration" interval;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "UpperAccessLimitPerTimeDuration_UpperLimit" bigint;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data ALTER COLUMN "Id" SET DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.get_https_resource ALTER COLUMN "Id" SET DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.geometric_data ALTER COLUMN "Id" SET DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data ALTER COLUMN "Id" SET DEFAULT (uuidv7());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD CONSTRAINT "PK_user_access_policy" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD CONSTRAINT "PK_open_id_connect_application_access_policy" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD CONSTRAINT "PK_institution_access_policy" PRIMARY KEY ("Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE TABLE database.data_access_policy (
        "Id" uuid NOT NULL DEFAULT (uuidv7()),
        "CalorimetricDataId" uuid,
        "GeometricDataId" uuid,
        "HygrothermalDataId" uuid,
        "LifeCycleDataId" uuid,
        "OpticalDataId" uuid,
        "PhotovoltaicDataId" uuid,
        "Combinator" database.logical_combinator NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now()),
        "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT "PK_data_access_policy" PRIMARY KEY ("Id"),
        CONSTRAINT "CK_DataAccessPolicy_At_Most_One_Data_Set" CHECK (NUM_NONNULLS("CalorimetricDataId", "GeometricDataId", "HygrothermalDataId", "LifeCycleDataId", "OpticalDataId", "PhotovoltaicDataId") <= 1),
        CONSTRAINT "FK_data_access_policy_calorimetric_data_CalorimetricDataId" FOREIGN KEY ("CalorimetricDataId") REFERENCES database.calorimetric_data ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_data_access_policy_geometric_data_GeometricDataId" FOREIGN KEY ("GeometricDataId") REFERENCES database.geometric_data ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_data_access_policy_hygrothermal_data_HygrothermalDataId" FOREIGN KEY ("HygrothermalDataId") REFERENCES database.hygrothermal_data ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_data_access_policy_lifeCycle_data_LifeCycleDataId" FOREIGN KEY ("LifeCycleDataId") REFERENCES database."lifeCycle_data" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_data_access_policy_optical_data_OpticalDataId" FOREIGN KEY ("OpticalDataId") REFERENCES database.optical_data ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_data_access_policy_photovoltaic_data_PhotovoltaicDataId" FOREIGN KEY ("PhotovoltaicDataId") REFERENCES database.photovoltaic_data ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_user_access_policy_CreatedAt_Id" ON database.user_access_policy ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_user_access_policy_DataAccessPolicyId_UserId" ON database.user_access_policy ("DataAccessPolicyId", "UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_open_id_connect_application_access_policy_CreatedAt_Id" ON database.open_id_connect_application_access_policy ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_open_id_connect_application_access_policy_DataAccessPolicyI~" ON database.open_id_connect_application_access_policy ("DataAccessPolicyId", "ClientId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_institution_access_policy_CreatedAt_Id" ON database.institution_access_policy ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_institution_access_policy_DataAccessPolicyId_InstitutionId" ON database.institution_access_policy ("DataAccessPolicyId", "InstitutionId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_data_access_policy_CalorimetricDataId" ON database.data_access_policy ("CalorimetricDataId") WHERE "CalorimetricDataId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_data_access_policy_CalorimetricDataId_GeometricDataId_Hygro~" ON database.data_access_policy ("CalorimetricDataId", "GeometricDataId", "HygrothermalDataId", "LifeCycleDataId", "OpticalDataId", "PhotovoltaicDataId") NULLS NOT DISTINCT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_data_access_policy_CreatedAt_Id" ON database.data_access_policy ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_data_access_policy_GeometricDataId" ON database.data_access_policy ("GeometricDataId") WHERE "GeometricDataId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_data_access_policy_HygrothermalDataId" ON database.data_access_policy ("HygrothermalDataId") WHERE "HygrothermalDataId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_data_access_policy_LifeCycleDataId" ON database.data_access_policy ("LifeCycleDataId") WHERE "LifeCycleDataId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_data_access_policy_OpticalDataId" ON database.data_access_policy ("OpticalDataId") WHERE "OpticalDataId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE UNIQUE INDEX "IX_data_access_policy_PhotovoltaicDataId" ON database.data_access_policy ("PhotovoltaicDataId") WHERE "PhotovoltaicDataId" IS NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD CONSTRAINT "FK_institution_access_policy_data_access_policy_DataAccessPoli~" FOREIGN KEY ("DataAccessPolicyId") REFERENCES database.data_access_policy ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD CONSTRAINT "FK_open_id_connect_application_access_policy_data_access_polic~" FOREIGN KEY ("DataAccessPolicyId") REFERENCES database.data_access_policy ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD CONSTRAINT "FK_user_access_policy_data_access_policy_DataAccessPolicyId" FOREIGN KEY ("DataAccessPolicyId") REFERENCES database.data_access_policy ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_data_access_policy_data_id_cannot_change"() RETURNS trigger as $LC_TRIGGER_data_access_policy_data_id_cannot_change$
    BEGIN
      IF COALESCE(OLD."CalorimetricDataId", OLD."GeometricDataId", OLD."HygrothermalDataId", OLD."LifeCycleDataId", OLD."OpticalDataId", OLD."PhotovoltaicDataId") <> COALESCE(NEW."CalorimetricDataId", NEW."GeometricDataId", NEW."HygrothermalDataId", NEW."LifeCycleDataId", NEW."OpticalDataId", NEW."PhotovoltaicDataId")
    THEN
        RAISE EXCEPTION 'You cannot change the data ID of a data access policy.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_data_access_policy_data_id_cannot_change$ LANGUAGE plpgsql;
    CREATE TRIGGER LC_TRIGGER_data_access_policy_data_id_cannot_change BEFORE UPDATE
    ON "database"."data_access_policy"
    FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_data_access_policy_data_id_cannot_change"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_get_https_resource_data_id_cannot_change"() RETURNS trigger as $LC_TRIGGER_get_https_resource_data_id_cannot_change$
    BEGIN
      IF COALESCE(OLD."CalorimetricDataId", OLD."GeometricDataId", OLD."HygrothermalDataId", OLD."LifeCycleDataId", OLD."OpticalDataId", OLD."PhotovoltaicDataId") <> COALESCE(NEW."CalorimetricDataId", NEW."GeometricDataId", NEW."HygrothermalDataId", NEW."LifeCycleDataId", NEW."OpticalDataId", NEW."PhotovoltaicDataId")
    THEN
        RAISE EXCEPTION 'You cannot change the data ID of a resource.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_get_https_resource_data_id_cannot_change$ LANGUAGE plpgsql;
    CREATE TRIGGER LC_TRIGGER_get_https_resource_data_id_cannot_change BEFORE UPDATE
    ON "database"."get_https_resource"
    FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_get_https_resource_data_id_cannot_change"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_get_https_resource_data_ids_must_match"() RETURNS trigger as $LC_TRIGGER_get_https_resource_data_ids_must_match$
    BEGIN
      IF NEW."ParentId" IS NOT NULL
       AND (
           SELECT COUNT("Id")
           FROM database."get_https_resource"
           WHERE "Id" = NEW."ParentId" AND COALESCE("CalorimetricDataId", "GeometricDataId", "HygrothermalDataId", "LifeCycleDataId", "OpticalDataId", "PhotovoltaicDataId") = COALESCE(NEW."CalorimetricDataId", NEW."GeometricDataId", NEW."HygrothermalDataId", NEW."LifeCycleDataId", NEW."OpticalDataId", NEW."PhotovoltaicDataId")
       )
       <> 1
    THEN
        RAISE EXCEPTION 'The new resource must have the same data ID as its parent.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_get_https_resource_data_ids_must_match$ LANGUAGE plpgsql;
    CREATE TRIGGER LC_TRIGGER_get_https_resource_data_ids_must_match BEFORE INSERT
    ON "database"."get_https_resource"
    FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_get_https_resource_data_ids_must_match"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260608173427_UnifyAccessPolicies', '10.0.8');
    END IF;
END $EF$;
COMMIT;

