START TRANSACTION;
ALTER TABLE database.photovoltaic_data ADD "Approval_ApproverId" uuid;

ALTER TABLE database.optical_data ADD "Approval_ApproverId" uuid;

ALTER TABLE database.hygrothermal_data ADD "Approval_ApproverId" uuid;

ALTER TABLE database.geometric_data ADD "Approval_ApproverId" uuid;

ALTER TABLE database.calorimetric_data ADD "Approval_ApproverId" uuid;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250907130041_AddApproverIdToResponseApproval', '9.0.8');

COMMIT;

