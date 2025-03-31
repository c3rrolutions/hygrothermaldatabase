START TRANSACTION;
DROP TABLE database.access_rights;

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Query";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Response";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.optical_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Query";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Response";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Query";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Response";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Query";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Response";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Timestamp";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_KeyFingerprint";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Query";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Response";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Signature";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Timestamp";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250331141152_AddAccessRights';

COMMIT;

