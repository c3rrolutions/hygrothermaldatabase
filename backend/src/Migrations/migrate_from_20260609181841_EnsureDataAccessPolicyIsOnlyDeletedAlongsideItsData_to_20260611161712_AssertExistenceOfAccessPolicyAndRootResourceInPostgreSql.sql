START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data"() CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
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
        RAISE EXCEPTION 'You cannot delete a data access policy without also deleting the corresponding data in the same transaction.';
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_calorimetric_data_assert_existence_of_root_resource"() RETURNS trigger as $LC_TRIGGER_calorimetric_data_assert_existence_of_root_resource$
    BEGIN
      IF NOT EXISTS (
        SELECT 1 FROM database."get_https_resource"
        WHERE "ParentId" IS NULL
        AND "CalorimetricDataId" = NEW."Id"
    )
    THEN
        RAISE EXCEPTION 'You cannot insert data without also inserting the corresponding root resource in the same transaction.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_calorimetric_data_assert_existence_of_root_resource$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_calorimetric_data_assert_existence_of_root_resource AFTER INSERT
    ON "database"."calorimetric_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_calorimetric_data_assert_existence_of_root_resource"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_create_calorimetric_data_access_policy_if_necessary"() RETURNS trigger as $LC_TRIGGER_create_calorimetric_data_access_policy_if_necessary$
    BEGIN
      INSERT INTO database."data_access_policy"
    ("CalorimetricDataId", "Combinator")
    VALUES (NEW."Id", 'all')
    ON CONFLICT ("CalorimetricDataId") WHERE "CalorimetricDataId" IS NOT NULL DO NOTHING;
    RETURN NEW;
    END;
    $LC_TRIGGER_create_calorimetric_data_access_policy_if_necessary$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_create_calorimetric_data_access_policy_if_necessary AFTER INSERT
    ON "database"."calorimetric_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_create_calorimetric_data_access_policy_if_necessary"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_create_geometric_data_access_policy_if_necessary"() RETURNS trigger as $LC_TRIGGER_create_geometric_data_access_policy_if_necessary$
    BEGIN
      INSERT INTO database."data_access_policy"
    ("GeometricDataId", "Combinator")
    VALUES (NEW."Id", 'all')
    ON CONFLICT ("GeometricDataId") WHERE "GeometricDataId" IS NOT NULL DO NOTHING;
    RETURN NEW;
    END;
    $LC_TRIGGER_create_geometric_data_access_policy_if_necessary$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_create_geometric_data_access_policy_if_necessary AFTER INSERT
    ON "database"."geometric_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_create_geometric_data_access_policy_if_necessary"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_geometric_data_assert_existence_of_root_resource"() RETURNS trigger as $LC_TRIGGER_geometric_data_assert_existence_of_root_resource$
    BEGIN
      IF NOT EXISTS (
        SELECT 1 FROM database."get_https_resource"
        WHERE "ParentId" IS NULL
        AND "GeometricDataId" = NEW."Id"
    )
    THEN
        RAISE EXCEPTION 'You cannot insert data without also inserting the corresponding root resource in the same transaction.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_geometric_data_assert_existence_of_root_resource$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_geometric_data_assert_existence_of_root_resource AFTER INSERT
    ON "database"."geometric_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_geometric_data_assert_existence_of_root_resource"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_get_https_resource_root_can_only_be_deleted_alongside_its_data"() RETURNS trigger as $LC_TRIGGER_get_https_resource_root_can_only_be_deleted_alongside_its_data$
    BEGIN
      IF OLD."ParentId" IS NULL AND (
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
        RAISE EXCEPTION 'You cannot delete a root resource without also deleting the corresponding data in the same transaction.';
    END IF;
    RETURN OLD;
    END;
    $LC_TRIGGER_get_https_resource_root_can_only_be_deleted_alongside_its_data$ LANGUAGE plpgsql;
    CREATE TRIGGER LC_TRIGGER_get_https_resource_root_can_only_be_deleted_alongside_its_data BEFORE DELETE
    ON "database"."get_https_resource"
    FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_get_https_resource_root_can_only_be_deleted_alongside_its_data"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_create_hygrothermal_data_access_policy_if_necessary"() RETURNS trigger as $LC_TRIGGER_create_hygrothermal_data_access_policy_if_necessary$
    BEGIN
      INSERT INTO database."data_access_policy"
    ("HygrothermalDataId", "Combinator")
    VALUES (NEW."Id", 'all')
    ON CONFLICT ("HygrothermalDataId") WHERE "HygrothermalDataId" IS NOT NULL DO NOTHING;
    RETURN NEW;
    END;
    $LC_TRIGGER_create_hygrothermal_data_access_policy_if_necessary$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_create_hygrothermal_data_access_policy_if_necessary AFTER INSERT
    ON "database"."hygrothermal_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_create_hygrothermal_data_access_policy_if_necessary"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_hygrothermal_data_assert_existence_of_root_resource"() RETURNS trigger as $LC_TRIGGER_hygrothermal_data_assert_existence_of_root_resource$
    BEGIN
      IF NOT EXISTS (
        SELECT 1 FROM database."get_https_resource"
        WHERE "ParentId" IS NULL
        AND "HygrothermalDataId" = NEW."Id"
    )
    THEN
        RAISE EXCEPTION 'You cannot insert data without also inserting the corresponding root resource in the same transaction.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_hygrothermal_data_assert_existence_of_root_resource$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_hygrothermal_data_assert_existence_of_root_resource AFTER INSERT
    ON "database"."hygrothermal_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_hygrothermal_data_assert_existence_of_root_resource"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_create_life_cycle_data_access_policy_if_necessary"() RETURNS trigger as $LC_TRIGGER_create_life_cycle_data_access_policy_if_necessary$
    BEGIN
      INSERT INTO database."data_access_policy"
    ("LifeCycleDataId", "Combinator")
    VALUES (NEW."Id", 'all')
    ON CONFLICT ("LifeCycleDataId") WHERE "LifeCycleDataId" IS NOT NULL DO NOTHING;
    RETURN NEW;
    END;
    $LC_TRIGGER_create_life_cycle_data_access_policy_if_necessary$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_create_life_cycle_data_access_policy_if_necessary AFTER INSERT
    ON "database"."lifeCycle_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_create_life_cycle_data_access_policy_if_necessary"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_life_cycle_data_assert_existence_of_root_resource"() RETURNS trigger as $LC_TRIGGER_life_cycle_data_assert_existence_of_root_resource$
    BEGIN
      IF NOT EXISTS (
        SELECT 1 FROM database."get_https_resource"
        WHERE "ParentId" IS NULL
        AND "LifeCycleDataId" = NEW."Id"
    )
    THEN
        RAISE EXCEPTION 'You cannot insert data without also inserting the corresponding root resource in the same transaction.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_life_cycle_data_assert_existence_of_root_resource$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_life_cycle_data_assert_existence_of_root_resource AFTER INSERT
    ON "database"."lifeCycle_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_life_cycle_data_assert_existence_of_root_resource"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_create_optical_data_access_policy_if_necessary"() RETURNS trigger as $LC_TRIGGER_create_optical_data_access_policy_if_necessary$
    BEGIN
      INSERT INTO database."data_access_policy"
    ("OpticalDataId", "Combinator")
    VALUES (NEW."Id", 'all')
    ON CONFLICT ("OpticalDataId") WHERE "OpticalDataId" IS NOT NULL DO NOTHING;
    RETURN NEW;
    END;
    $LC_TRIGGER_create_optical_data_access_policy_if_necessary$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_create_optical_data_access_policy_if_necessary AFTER INSERT
    ON "database"."optical_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_create_optical_data_access_policy_if_necessary"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_optical_data_assert_existence_of_root_resource"() RETURNS trigger as $LC_TRIGGER_optical_data_assert_existence_of_root_resource$
    BEGIN
      IF NOT EXISTS (
        SELECT 1 FROM database."get_https_resource"
        WHERE "ParentId" IS NULL
        AND "OpticalDataId" = NEW."Id"
    )
    THEN
        RAISE EXCEPTION 'You cannot insert data without also inserting the corresponding root resource in the same transaction.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_optical_data_assert_existence_of_root_resource$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_optical_data_assert_existence_of_root_resource AFTER INSERT
    ON "database"."optical_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_optical_data_assert_existence_of_root_resource"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_create_photovoltaic_data_access_policy_if_necessary"() RETURNS trigger as $LC_TRIGGER_create_photovoltaic_data_access_policy_if_necessary$
    BEGIN
      INSERT INTO database."data_access_policy"
    ("PhotovoltaicDataId", "Combinator")
    VALUES (NEW."Id", 'all')
    ON CONFLICT ("PhotovoltaicDataId") WHERE "PhotovoltaicDataId" IS NOT NULL DO NOTHING;
    RETURN NEW;
    END;
    $LC_TRIGGER_create_photovoltaic_data_access_policy_if_necessary$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_create_photovoltaic_data_access_policy_if_necessary AFTER INSERT
    ON "database"."photovoltaic_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_create_photovoltaic_data_access_policy_if_necessary"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_photovoltaic_data_assert_existence_of_root_resource"() RETURNS trigger as $LC_TRIGGER_photovoltaic_data_assert_existence_of_root_resource$
    BEGIN
      IF NOT EXISTS (
        SELECT 1 FROM database."get_https_resource"
        WHERE "ParentId" IS NULL
        AND "PhotovoltaicDataId" = NEW."Id"
    )
    THEN
        RAISE EXCEPTION 'You cannot insert data without also inserting the corresponding root resource in the same transaction.';
    END IF;
    RETURN NEW;
    END;
    $LC_TRIGGER_photovoltaic_data_assert_existence_of_root_resource$ LANGUAGE plpgsql;
    CREATE CONSTRAINT TRIGGER LC_TRIGGER_photovoltaic_data_assert_existence_of_root_resource AFTER INSERT
    ON "database"."photovoltaic_data"
    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE PROCEDURE "database"."LC_TRIGGER_photovoltaic_data_assert_existence_of_root_resource"();
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260611161712_AssertExistenceOfAccessPolicyAndRootResourceInPostgreSql', '10.0.8');
    END IF;
END $EF$;
COMMIT;

