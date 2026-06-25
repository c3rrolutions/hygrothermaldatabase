START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_access_policy_data_id_cannot_change"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP FUNCTION "database"."LC_TRIGGER_get_https_resource_data_id_cannot_change"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP FUNCTION "database"."LC_TRIGGER_get_https_resource_data_ids_must_match"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP CONSTRAINT "FK_institution_access_policy_data_access_policy_DataAccessPoli~";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP CONSTRAINT "FK_open_id_connect_application_access_policy_data_access_polic~";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP CONSTRAINT "FK_user_access_policy_data_access_policy_DataAccessPolicyId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP TABLE database.data_access_policy;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP CONSTRAINT "PK_user_access_policy";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP INDEX database."IX_user_access_policy_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP INDEX database."IX_user_access_policy_DataAccessPolicyId_UserId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP CONSTRAINT "PK_open_id_connect_application_access_policy";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP INDEX database."IX_open_id_connect_application_access_policy_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP INDEX database."IX_open_id_connect_application_access_policy_DataAccessPolicyI~";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP CONSTRAINT "PK_institution_access_policy";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP INDEX database."IX_institution_access_policy_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DROP INDEX database."IX_institution_access_policy_DataAccessPolicyId_InstitutionId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "AccessCountSinceStartTime_AccessCount";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "AccessCountSinceStartTime_StartTime";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "CreatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "DataAccessPolicyId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration_Duration";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration_UpperLimit";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "AccessCountSinceStartTime_AccessCount";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "AccessCountSinceStartTime_StartTime";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "CreatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "DataAccessPolicyId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration_Duration";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration_UpperLimit";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "AccessCountSinceStartTime_AccessCount";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "AccessCountSinceStartTime_StartTime";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "CreatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "DataAccessPolicyId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration_Duration";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy DROP COLUMN "UpperAccessLimitPerTimeDuration_UpperLimit";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "AccessCountSinceStartTime" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD "UpperAccessLimitPerTimeDuration" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database."user" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.photovoltaic_data ADD "AccessPolicy" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.optical_data ADD "AccessPolicy" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "AccessCountSinceStartTime" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD "UpperAccessLimitPerTimeDuration" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database."lifeCycle_data" ADD "AccessPolicy" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "AccessCountSinceStartTime" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD "UpperAccessLimitPerTimeDuration" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.hygrothermal_data ADD "AccessPolicy" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.get_https_resource ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.geometric_data ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.geometric_data ADD "AccessPolicy" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data ALTER COLUMN "Id" SET DEFAULT (gen_random_uuid());
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.calorimetric_data ADD "AccessPolicy" jsonb;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.user_access_policy ADD CONSTRAINT "PK_user_access_policy" PRIMARY KEY ("UserId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.open_id_connect_application_access_policy ADD CONSTRAINT "PK_open_id_connect_application_access_policy" PRIMARY KEY ("ClientId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    ALTER TABLE database.institution_access_policy ADD CONSTRAINT "PK_institution_access_policy" PRIMARY KEY ("InstitutionId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_data_id_cannot_change"() RETURNS trigger as $LC_TRIGGER_data_id_cannot_change$
    BEGIN
      IF COALESCE(OLD."CalorimetricDataId", OLD."GeometricDataId", OLD."HygrothermalDataId", OLD."LifeCycleDataId", OLD."OpticalDataId", OLD."PhotovoltaicDataId") <> COALESCE(NEW."CalorimetricDataId", NEW."GeometricDataId", NEW."HygrothermalDataId", NEW."LifeCycleDataId", NEW."OpticalDataId", NEW."PhotovoltaicDataId")
    THEN
        RAISE EXCEPTION 'You cannot change the data ID of a resource.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_data_id_cannot_change$ LANGUAGE plpgsql;
    CREATE TRIGGER LC_TRIGGER_data_id_cannot_change BEFORE UPDATE
    ON "database"."get_https_resource"
    FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_data_id_cannot_change"();
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_data_ids_must_match"() RETURNS trigger as $LC_TRIGGER_data_ids_must_match$
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
    $LC_TRIGGER_data_ids_must_match$ LANGUAGE plpgsql;
    CREATE TRIGGER LC_TRIGGER_data_ids_must_match BEFORE INSERT
    ON "database"."get_https_resource"
    FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_data_ids_must_match"();
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260608173427_UnifyAccessPolicies';
    END IF;
END $EF$;
COMMIT;

