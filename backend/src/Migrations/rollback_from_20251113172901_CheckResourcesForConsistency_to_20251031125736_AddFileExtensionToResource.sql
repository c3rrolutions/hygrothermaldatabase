START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_id_cannot_change"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_ids_must_match"() CASCADE;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_calorimetric_data_CalorimetricDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_geometric_data_GeometricDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_hygrothermal_data_HygrothermalDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_optical_data_OpticalDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_CalorimetricDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_GeometricDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_HygrothermalDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_OpticalDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DROP INDEX database."IX_get_https_resource_PhotovoltaicDataId";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "CK_GetHttpsResource_Exactly_One_Data_Set";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource DROP CONSTRAINT "CK_GetHttpsResource_Root_Or_Child";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    UPDATE database.photovoltaic_data SET "UserId" = '00000000-0000-0000-0000-000000000000' WHERE "UserId" IS NULL;
    ALTER TABLE database.photovoltaic_data ALTER COLUMN "UserId" SET NOT NULL;
    ALTER TABLE database.photovoltaic_data ALTER COLUMN "UserId" SET DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database."optical_data_Approvals" ALTER COLUMN "Variables" DROP DEFAULT;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    UPDATE database.optical_data SET "UserId" = '00000000-0000-0000-0000-000000000000' WHERE "UserId" IS NULL;
    ALTER TABLE database.optical_data ALTER COLUMN "UserId" SET NOT NULL;
    ALTER TABLE database.optical_data ALTER COLUMN "UserId" SET DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "Approval_Variables" DROP DEFAULT;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    UPDATE database.hygrothermal_data SET "UserId" = '00000000-0000-0000-0000-000000000000' WHERE "UserId" IS NULL;
    ALTER TABLE database.hygrothermal_data ALTER COLUMN "UserId" SET NOT NULL;
    ALTER TABLE database.hygrothermal_data ALTER COLUMN "UserId" SET DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    UPDATE database.geometric_data SET "UserId" = '00000000-0000-0000-0000-000000000000' WHERE "UserId" IS NULL;
    ALTER TABLE database.geometric_data ALTER COLUMN "UserId" SET NOT NULL;
    ALTER TABLE database.geometric_data ALTER COLUMN "UserId" SET DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    UPDATE database.calorimetric_data SET "UserId" = '00000000-0000-0000-0000-000000000000' WHERE "UserId" IS NULL;
    ALTER TABLE database.calorimetric_data ALTER COLUMN "UserId" SET NOT NULL;
    ALTER TABLE database.calorimetric_data ALTER COLUMN "UserId" SET DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE INDEX "IX_get_https_resource_CalorimetricDataId" ON database.get_https_resource ("CalorimetricDataId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE INDEX "IX_get_https_resource_GeometricDataId" ON database.get_https_resource ("GeometricDataId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE INDEX "IX_get_https_resource_HygrothermalDataId" ON database.get_https_resource ("HygrothermalDataId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE INDEX "IX_get_https_resource_OpticalDataId" ON database.get_https_resource ("OpticalDataId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    CREATE INDEX "IX_get_https_resource_PhotovoltaicDataId" ON database.get_https_resource ("PhotovoltaicDataId");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_calorimetric_data_CalorimetricDataId" FOREIGN KEY ("CalorimetricDataId") REFERENCES database.calorimetric_data ("Id");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_geometric_data_GeometricDataId" FOREIGN KEY ("GeometricDataId") REFERENCES database.geometric_data ("Id");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_hygrothermal_data_HygrothermalDataId" FOREIGN KEY ("HygrothermalDataId") REFERENCES database.hygrothermal_data ("Id");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_optical_data_OpticalDataId" FOREIGN KEY ("OpticalDataId") REFERENCES database.optical_data ("Id");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    ALTER TABLE database.get_https_resource ADD CONSTRAINT "FK_get_https_resource_photovoltaic_data_PhotovoltaicDataId" FOREIGN KEY ("PhotovoltaicDataId") REFERENCES database.photovoltaic_data ("Id");
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20251113172901_CheckResourcesForConsistency';
    END IF;
END $EF$;
COMMIT;

