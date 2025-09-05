START TRANSACTION;
ALTER TABLE database."photovoltaic_data_Approvals" ADD "Variables" jsonb NOT NULL DEFAULT ('{}');

ALTER TABLE database.photovoltaic_data ADD "Approval_Variables" jsonb DEFAULT ('{}');

ALTER TABLE database."optical_data_Approvals" ADD "Variables" jsonb NOT NULL DEFAULT ('{}');

ALTER TABLE database.optical_data ADD "Approval_Variables" jsonb DEFAULT ('{}');

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Variables" jsonb NOT NULL DEFAULT ('{}');

ALTER TABLE database.hygrothermal_data ADD "Approval_Variables" jsonb DEFAULT ('{}');

ALTER TABLE database."geometric_data_Approvals" ADD "Variables" jsonb NOT NULL DEFAULT ('{}');

ALTER TABLE database.geometric_data ADD "Approval_Variables" jsonb DEFAULT ('{}');

ALTER TABLE database."calorimetric_data_Approvals" ADD "Variables" jsonb NOT NULL DEFAULT ('{}');

ALTER TABLE database.calorimetric_data ADD "Approval_Variables" jsonb DEFAULT ('{}');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250812171810_AddQueryVariablesToApprovals', '9.0.8');

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Response" TO "Message";

ALTER TABLE database.photovoltaic_data RENAME COLUMN "Approval_Response" TO "Approval_Message";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Response" TO "Message";

ALTER TABLE database.optical_data RENAME COLUMN "Approval_Response" TO "Approval_Message";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Response" TO "Message";

ALTER TABLE database.hygrothermal_data RENAME COLUMN "Approval_Response" TO "Approval_Message";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Response" TO "Message";

ALTER TABLE database.geometric_data RENAME COLUMN "Approval_Response" TO "Approval_Message";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Response" TO "Message";

ALTER TABLE database.calorimetric_data RENAME COLUMN "Approval_Response" TO "Approval_Message";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250902122837_RenameResponsePropertyOfApprovalsToMessage', '9.0.8');

COMMIT;

