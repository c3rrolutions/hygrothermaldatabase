START TRANSACTION;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125181102_MirrorWidthAndHeightOfGeometricData') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "Heights";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125181102_MirrorWidthAndHeightOfGeometricData') THEN
    ALTER TABLE database.geometric_data DROP COLUMN "Widths";
    END IF;
END $EF$;
DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251125181102_MirrorWidthAndHeightOfGeometricData') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20251125181102_MirrorWidthAndHeightOfGeometricData';
    END IF;
END $EF$;
COMMIT;

