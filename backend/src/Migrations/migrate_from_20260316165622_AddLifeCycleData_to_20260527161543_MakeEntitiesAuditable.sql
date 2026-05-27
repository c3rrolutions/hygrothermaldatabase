START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database."user" ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database."user" ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.photovoltaic_data ALTER COLUMN "CreatedAt" SET DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.photovoltaic_data ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.optical_data ALTER COLUMN "CreatedAt" SET DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.optical_data ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database."lifeCycle_data" ALTER COLUMN "CreatedAt" SET DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database."lifeCycle_data" ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.institution_access_rights ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.institution_access_rights ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.hygrothermal_data ALTER COLUMN "CreatedAt" SET DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.hygrothermal_data ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.get_https_resource ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.get_https_resource ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.geometric_data ALTER COLUMN "CreatedAt" SET DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.geometric_data ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.calorimetric_data ALTER COLUMN "CreatedAt" SET DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    ALTER TABLE database.calorimetric_data ADD "UpdatedAt" timestamp with time zone NOT NULL DEFAULT (now());
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_user_CreatedAt_Id" ON database."user" ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_photovoltaic_data_CreatedAt_Id" ON database.photovoltaic_data ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_optical_data_CreatedAt_Id" ON database.optical_data ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_lifeCycle_data_CreatedAt_Id" ON database."lifeCycle_data" ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_institution_access_rights_CreatedAt_Id" ON database.institution_access_rights ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_hygrothermal_data_CreatedAt_Id" ON database.hygrothermal_data ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_get_https_resource_CreatedAt_Id" ON database.get_https_resource ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_geometric_data_CreatedAt_Id" ON database.geometric_data ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    CREATE UNIQUE INDEX "IX_calorimetric_data_CreatedAt_Id" ON database.calorimetric_data ("CreatedAt", "Id");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260527161543_MakeEntitiesAuditable') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260527161543_MakeEntitiesAuditable', '10.0.8');
    END IF;
END $EF$;
COMMIT;

