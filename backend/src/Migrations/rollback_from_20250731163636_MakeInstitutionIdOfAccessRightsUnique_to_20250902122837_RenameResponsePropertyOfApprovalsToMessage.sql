START TRANSACTION;
ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Message" TO "Response";

ALTER TABLE database.photovoltaic_data RENAME COLUMN "Approval_Message" TO "Approval_Response";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Message" TO "Response";

ALTER TABLE database.optical_data RENAME COLUMN "Approval_Message" TO "Approval_Response";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Message" TO "Response";

ALTER TABLE database.hygrothermal_data RENAME COLUMN "Approval_Message" TO "Approval_Response";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Message" TO "Response";

ALTER TABLE database.geometric_data RENAME COLUMN "Approval_Message" TO "Approval_Response";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Message" TO "Response";

ALTER TABLE database.calorimetric_data RENAME COLUMN "Approval_Message" TO "Approval_Response";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250902122837_RenameResponsePropertyOfApprovalsToMessage';

ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Variables";

ALTER TABLE database.photovoltaic_data DROP COLUMN "Approval_Variables";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Variables";

ALTER TABLE database.optical_data DROP COLUMN "Approval_Variables";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Variables";

ALTER TABLE database.hygrothermal_data DROP COLUMN "Approval_Variables";

ALTER TABLE database."geometric_data_Approvals" DROP COLUMN "Variables";

ALTER TABLE database.geometric_data DROP COLUMN "Approval_Variables";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Variables";

ALTER TABLE database.calorimetric_data DROP COLUMN "Approval_Variables";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250812171810_AddQueryVariablesToApprovals';

COMMIT;

