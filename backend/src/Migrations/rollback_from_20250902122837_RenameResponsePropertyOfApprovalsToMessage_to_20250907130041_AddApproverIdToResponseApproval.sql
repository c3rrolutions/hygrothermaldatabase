START TRANSACTION;
ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_ApproverId";

ALTER TABLE database.optical_data DROP COLUMN "Approval_ApproverId";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_ApproverId";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_ApproverId";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_ApproverId";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250907130041_AddApproverIdToResponseApproval';

COMMIT;

