START TRANSACTION;
UPDATE database.get_https_resource SET "Description" = '' WHERE "Description" IS NULL;
ALTER TABLE database.get_https_resource ALTER COLUMN "Description" SET NOT NULL;
ALTER TABLE database.get_https_resource ALTER COLUMN "Description" SET DEFAULT '';

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250218172859_MakeDescriptionOptional';

ALTER TABLE database.optical_data DROP COLUMN "CoatedSide";

ALTER TABLE database.optical_data DROP COLUMN "Subtype";

ALTER TABLE database.optical_data DROP COLUMN "Type";

DROP TYPE database.coated_side;
DROP TYPE database.optical_component_subtype;
DROP TYPE database.optical_component_type;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250213155333_AddTypeSubtypeAndCoatedSideToData';

ALTER TABLE database."geometric_data_Sources" ALTER COLUMN "Value_DataKind" TYPE data_kind;

ALTER TABLE database."geometric_data_Approvals" ALTER COLUMN "Standard_Standardizers" TYPE standardizer[];

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250129163501_UpgradeToNet9';

COMMIT;

