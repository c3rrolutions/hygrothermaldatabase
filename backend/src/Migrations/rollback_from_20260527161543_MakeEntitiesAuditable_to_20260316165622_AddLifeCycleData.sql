START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_user_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_photovoltaic_data_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_optical_data_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_lifeCycle_data_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_institution_access_rights_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_hygrothermal_data_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_get_https_resource_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_geometric_data_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DROP INDEX database."IX_calorimetric_data_CreatedAt_Id";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database."user" DROP COLUMN "CreatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database."user" DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.photovoltaic_data DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.optical_data DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database."lifeCycle_data" DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.institution_access_rights DROP COLUMN "CreatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.institution_access_rights DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.hygrothermal_data DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.get_https_resource DROP COLUMN "CreatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.get_https_resource DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.calorimetric_data DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.photovoltaic_data ALTER COLUMN "CreatedAt" DROP DEFAULT;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "CreatedAt" DROP DEFAULT;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database."lifeCycle_data" ALTER COLUMN "CreatedAt" DROP DEFAULT;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.hygrothermal_data ALTER COLUMN "CreatedAt" DROP DEFAULT;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.geometric_data ALTER COLUMN "CreatedAt" DROP DEFAULT;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.calorimetric_data ALTER COLUMN "CreatedAt" DROP DEFAULT;
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable';
    END IF;
END $EF$;
COMMIT;

