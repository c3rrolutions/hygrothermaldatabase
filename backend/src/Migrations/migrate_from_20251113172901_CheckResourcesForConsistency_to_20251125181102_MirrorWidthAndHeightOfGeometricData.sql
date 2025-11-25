START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125181102_MirrorWidthAndHeightOfGeometricData') THEN
    ALTER TABLE database.geometric_data ADD "Heights" double precision[] NOT NULL DEFAULT ARRAY[]::double precision[];
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125181102_MirrorWidthAndHeightOfGeometricData') THEN
    ALTER TABLE database.geometric_data ADD "Widths" double precision[] NOT NULL DEFAULT ARRAY[]::double precision[];
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125181102_MirrorWidthAndHeightOfGeometricData') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251125181102_MirrorWidthAndHeightOfGeometricData', '9.0.10');
    END IF;
END $EF$;
COMMIT;

