START TRANSACTION;
ALTER TABLE database."geometric_data_Sources" ALTER COLUMN "Value_DataKind" TYPE database.data_kind;

ALTER TABLE database."geometric_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE database.standardizer[];

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250129163501_UpgradeToNet9', '9.0.1');

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE TYPE database.coated_side AS ENUM ('back', 'both', 'front', 'neither');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE TYPE database.optical_component_subtype AS ENUM ('acid_etched_glass', 'applied_film', 'cellular_shade', 'chromogenic', 'coated', 'coating', 'diffusing_shade', 'embedded_coating', 'film', 'fritted_glass', 'interlayer', 'laminate', 'monolithic', 'perforated_screen', 'pleated_shade', 'roller_shade', 'roman_shade', 'sandblasted_glass', 'shade_material', 'venetian_blind', 'vertical_louver', 'woven_shade');
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'database') THEN
        CREATE SCHEMA database;
    END IF;
END $EF$;

CREATE TYPE database.optical_component_type AS ENUM ('glazing', 'shading');

ALTER TABLE database.optical_data ADD "CoatedSide" database.coated_side;

ALTER TABLE database.optical_data ADD "Subtype" database.optical_component_subtype;

ALTER TABLE database.optical_data ADD "Type" database.optical_component_type;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250213155333_AddTypeSubtypeAndCoatedSideToData', '9.0.1');

ALTER TABLE database.get_https_resource ALTER COLUMN "Description" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250218172859_MakeDescriptionOptional', '9.0.1');

COMMIT;

