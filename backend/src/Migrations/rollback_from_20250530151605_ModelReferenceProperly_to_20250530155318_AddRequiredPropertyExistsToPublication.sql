START TRANSACTION;
ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Statement_Publication_Exists";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Statement_Publication_Exists";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Statement_Publication_Exists";

ALTER TABLE database."geometric_data_Approvals" DROP COLUMN "Statement_Publication_Exists";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Statement_Publication_Exists";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250530155318_AddRequiredPropertyExistsToPublication';

COMMIT;

