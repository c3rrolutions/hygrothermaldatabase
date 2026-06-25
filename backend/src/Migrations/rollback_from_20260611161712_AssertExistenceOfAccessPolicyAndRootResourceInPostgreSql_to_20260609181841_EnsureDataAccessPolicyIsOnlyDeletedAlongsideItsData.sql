START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_calorimetric_data_assert_existence_of_root_resource"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_create_calorimetric_data_access_policy_if_necessary"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_create_geometric_data_access_policy_if_necessary"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_geometric_data_assert_existence_of_root_resource"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_get_https_resource_root_can_only_be_deleted_alongside_its_data"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_create_hygrothermal_data_access_policy_if_necessary"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_hygrothermal_data_assert_existence_of_root_resource"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_create_life_cycle_data_access_policy_if_necessary"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_life_cycle_data_assert_existence_of_root_resource"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_create_optical_data_access_policy_if_necessary"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_optical_data_assert_existence_of_root_resource"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_create_photovoltaic_data_access_policy_if_necessary"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_photovoltaic_data_assert_existence_of_root_resource"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
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
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql';
    END IF;
END $EF$;
COMMIT;

