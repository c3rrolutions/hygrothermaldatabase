START TRANSACTION;
ALTER TABLE database."photovoltaic_data_Approvals" ADD "Statement_Publication_Exists" boolean;

ALTER TABLE database."optical_data_Approvals" ADD "Statement_Publication_Exists" boolean;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Statement_Publication_Exists" boolean;

ALTER TABLE database."geometric_data_Approvals" ADD "Statement_Publication_Exists" boolean;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Statement_Publication_Exists" boolean;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250530155318_AddRequiredPropertyExistsToPublication', '9.0.5');

COMMIT;

