START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData') THEN
        DELETE FROM database.data_access_policy;
        DELETE FROM database.user_access_policy;
        DELETE FROM database.institution_access_policy;
        DELETE FROM database.open_id_connect_application_access_policy;

        INSERT INTO database.data_access_policy ("Combinator") VALUES ('all'::database.logical_combinator);

        INSERT INTO database.data_access_policy ("CalorimetricDataId", "Combinator")
            SELECT "Id", 'all'::database.logical_combinator
            FROM database."calorimetric_data";

        INSERT INTO database.data_access_policy ("GeometricDataId", "Combinator")
            SELECT "Id", 'all'::database.logical_combinator
            FROM database."geometric_data";

        INSERT INTO database.data_access_policy ("HygrothermalDataId", "Combinator")
            SELECT "Id", 'all'::database.logical_combinator
            FROM database."hygrothermal_data";

        INSERT INTO database.data_access_policy ("LifeCycleDataId", "Combinator")
            SELECT "Id", 'all'::database.logical_combinator
            FROM database."lifeCycle_data";

        INSERT INTO database.data_access_policy ("OpticalDataId", "Combinator")
            SELECT "Id", 'all'::database.logical_combinator
            FROM database."optical_data";

        INSERT INTO database.data_access_policy ("PhotovoltaicDataId", "Combinator")
            SELECT "Id", 'all'::database.logical_combinator
            FROM database."photovoltaic_data";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data"() RETURNS trigger as $LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data$
    BEGIN
      IF (
        (OLD."CalorimetricDataId" IS NOT NULL AND EXISTS (
        SELECT 1 FROM database."calorimetric_data" 
        WHERE "Id" = OLD."CalorimetricDataId"
    )) OR (OLD."GeometricDataId" IS NOT NULL AND EXISTS (
        SELECT 1 FROM database."geometric_data" 
        WHERE "Id" = OLD."GeometricDataId"
    )) OR (OLD."HygrothermalDataId" IS NOT NULL AND EXISTS (
        SELECT 1 FROM database."hygrothermal_data" 
        WHERE "Id" = OLD."HygrothermalDataId"
    )) OR (OLD."LifeCycleDataId" IS NOT NULL AND EXISTS (
        SELECT 1 FROM database."lifeCycle_data" 
        WHERE "Id" = OLD."LifeCycleDataId"
    )) OR (OLD."OpticalDataId" IS NOT NULL AND EXISTS (
        SELECT 1 FROM database."optical_data" 
        WHERE "Id" = OLD."OpticalDataId"
    )) OR (OLD."PhotovoltaicDataId" IS NOT NULL AND EXISTS (
        SELECT 1 FROM database."photovoltaic_data" 
        WHERE "Id" = OLD."PhotovoltaicDataId"
    ))
    ) THEN
        RAISE EXCEPTION 'You cannot delete a data access policy without also deleting the corresponding data.';
    END IF;
    RETURN OLD;
    END;
    $LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data$ LANGUAGE plpgsql;
    CREATE TRIGGER LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data BEFORE DELETE
    ON "database"."data_access_policy"
    FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted"() RETURNS trigger as $LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted$
    BEGIN
      IF OLD."CalorimetricDataId" IS NULL AND OLD."GeometricDataId" IS NULL AND OLD."HygrothermalDataId" IS NULL AND OLD."LifeCycleDataId" IS NULL AND OLD."OpticalDataId" IS NULL AND OLD."PhotovoltaicDataId" IS NULL
    THEN
        RAISE EXCEPTION 'You cannot delete the global data access policy.';
    END IF;
    RETURN OLD;
    END;
    $LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted$ LANGUAGE plpgsql;
    CREATE TRIGGER LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted BEFORE DELETE
    ON "database"."data_access_policy"
    FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData', '10.0.8');
    END IF;
END $EF$;
COMMIT;

