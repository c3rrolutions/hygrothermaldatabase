START TRANSACTION;
DROP TABLE database.access_rights;

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Query";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Response";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.photovoltaic_data DROP COLUMN "DataAccess";

ALTER TABLE database.optical_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Query";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Response";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.optical_data DROP COLUMN "DataAccess";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Query";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Response";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.hygrothermal_data DROP COLUMN "DataAccess";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Query";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Response";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.geometric_data DROP COLUMN "DataAccess";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Query";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Response";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.calorimetric_data DROP COLUMN "DataAccess";

UPDATE database.get_https_resource SET "Description" = '' WHERE "Description" IS NULL;
ALTER TABLE database.get_https_resource ALTER COLUMN "Description" SET NOT NULL;
ALTER TABLE database.get_https_resource ALTER COLUMN "Description" SET DEFAULT '';

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250319132121_AddAccessRights';

COMMIT;

