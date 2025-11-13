START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_calorimetric_data_CalorimetricDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_geometric_data_GeometricDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_hygrothermal_data_HygrothermalDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_optical_data_OpticalDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_CalorimetricDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_GeometricDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_HygrothermalDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_OpticalDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_PhotovoltaicDataId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.photovoltaic_data ALTER COLUMN "UserId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database."optical_data_Approvals" ALTER COLUMN "Variables" SET DEFAULT ('{}');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "UserId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "Approval_Variables" SET DEFAULT ('{}');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.hygrothermal_data ALTER COLUMN "UserId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.geometric_data ALTER COLUMN "UserId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.calorimetric_data ALTER COLUMN "UserId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE UNIQUE INDEX "IX_get_https_resource_CalorimetricDataId" ON database.get_https_resource ("CalorimetricDataId") WHERE "CalorimetricDataId" IS NOT NULL AND "ParentId" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE UNIQUE INDEX "IX_get_https_resource_GeometricDataId" ON database.get_https_resource ("GeometricDataId") WHERE "GeometricDataId" IS NOT NULL AND "ParentId" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE UNIQUE INDEX "IX_get_https_resource_HygrothermalDataId" ON database.get_https_resource ("HygrothermalDataId") WHERE "HygrothermalDataId" IS NOT NULL AND "ParentId" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE UNIQUE INDEX "IX_get_https_resource_OpticalDataId" ON database.get_https_resource ("OpticalDataId") WHERE "OpticalDataId" IS NOT NULL AND "ParentId" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE UNIQUE INDEX "IX_get_https_resource_PhotovoltaicDataId" ON database.get_https_resource ("PhotovoltaicDataId") WHERE "PhotovoltaicDataId" IS NOT NULL AND "ParentId" IS NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "CK_GetHttpsResource_Exactly_One_Data_Set" CHECK (NUM_NONNULLS("CalorimetricDataId", "GeometricDataId", "HygrothermalDataId", "OpticalDataId", "PhotovoltaicDataId") = 1);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "CK_GetHttpsResource_Root_Or_Child" CHECK (("ParentId" IS NULL AND "AppliedConversionMethod_MethodId" IS NULL)
    OR ("ParentId" IS NOT NULL AND "AppliedConversionMethod_MethodId" IS NOT NULL));
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_calorimetric_data_CalorimetricDataId" FOREIGN KEY ("CalorimetricDataId") REFERENCES database.calorimetric_data ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_geometric_data_GeometricDataId" FOREIGN KEY ("GeometricDataId") REFERENCES database.geometric_data ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_hygrothermal_data_HygrothermalDataId" FOREIGN KEY ("HygrothermalDataId") REFERENCES database.hygrothermal_data ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_optical_data_OpticalDataId" FOREIGN KEY ("OpticalDataId") REFERENCES database.optical_data ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId" FOREIGN KEY ("PhotovoltaicDataId") REFERENCES database.photovoltaic_data ("Id") ON DELETE CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_data_id_cannot_change"() RETURNS trigger as $LC_TRIGGER_data_id_cannot_change$
    BEGIN
      IF COALESCE(OLD."CalorimetricDataId", OLD."GeometricDataId", OLD."HygrothermalDataId", OLD."OpticalDataId", OLD."PhotovoltaicDataId") <> COALESCE(NEW."CalorimetricDataId", NEW."GeometricDataId", NEW."HygrothermalDataId", NEW."OpticalDataId", NEW."PhotovoltaicDataId")
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE FUNCTION "database"."LC_TRIGGER_data_ids_must_match"() RETURNS trigger as $LC_TRIGGER_data_ids_must_match$
    BEGIN
      IF NEW."ParentId" IS NOT NULL
       AND (
           SELECT COUNT("Id")
           FROM database."get_https_resource"
           WHERE "Id" = NEW."ParentId" AND COALESCE("CalorimetricDataId", "GeometricDataId", "HygrothermalDataId", "OpticalDataId", "PhotovoltaicDataId") = COALESCE(NEW."CalorimetricDataId", NEW."GeometricDataId", NEW."HygrothermalDataId", NEW."OpticalDataId", NEW."PhotovoltaicDataId")
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
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251113172901_CheckResourcesForConsistency', '9.0.10');
    END IF;
END $EF$;
COMMIT;

