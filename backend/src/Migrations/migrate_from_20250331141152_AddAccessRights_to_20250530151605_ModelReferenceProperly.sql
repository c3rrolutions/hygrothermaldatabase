START TRANSACTION;
ALTER TABLE database.photovoltaic_data ADD "DataAccessRights_AllowedApplications" text[];

ALTER TABLE database.photovoltaic_data ADD "DataAccessRights_AllowedInstitutions" uuid[];

ALTER TABLE database.photovoltaic_data ADD "DataAccessRights_AllowedUserAndQuantity" jsonb;

ALTER TABLE database.optical_data ADD "DataAccessRights_AllowedApplications" text[];

ALTER TABLE database.optical_data ADD "DataAccessRights_AllowedInstitutions" uuid[];

ALTER TABLE database.optical_data ADD "DataAccessRights_AllowedUserAndQuantity" jsonb;

ALTER TABLE database.hygrothermal_data ADD "DataAccessRights_AllowedApplications" text[];

ALTER TABLE database.hygrothermal_data ADD "DataAccessRights_AllowedInstitutions" uuid[];

ALTER TABLE database.hygrothermal_data ADD "DataAccessRights_AllowedUserAndQuantity" jsonb;

ALTER TABLE database.geometric_data ADD "DataAccessRights_AllowedApplications" text[];

ALTER TABLE database.geometric_data ADD "DataAccessRights_AllowedInstitutions" uuid[];

ALTER TABLE database.geometric_data ADD "DataAccessRights_AllowedUserAndQuantity" jsonb;

ALTER TABLE database.calorimetric_data ADD "DataAccessRights_AllowedApplications" text[];

ALTER TABLE database.calorimetric_data ADD "DataAccessRights_AllowedInstitutions" uuid[];

ALTER TABLE database.calorimetric_data ADD "DataAccessRights_AllowedUserAndQuantity" jsonb;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250527144412_AddDataAccessRights', '9.0.5');

ALTER TABLE database.access_rights DROP CONSTRAINT "PK_access_rights";

ALTER TABLE database.access_rights RENAME TO institution_access_rights;

ALTER TABLE database.institution_access_rights ADD CONSTRAINT "PK_institution_access_rights" PRIMARY KEY ("Id");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250527150325_MoveInstitutionAccessRights', '9.0.5');

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Year" TO "Statement_Standard_Year";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Title" TO "Statement_Standard_Title";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Standardizers" TO "Statement_Standard_Standardizers";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Section" TO "Statement_Standard_Section";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Numeration_Suffix" TO "Statement_Standard_Numeration_Suffix";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Numeration_Prefix" TO "Statement_Standard_Numeration_Prefix";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Numeration_MainNumber" TO "Statement_Standard_Numeration_MainNumber";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Locator" TO "Statement_Standard_Locator";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Standard_Abstract" TO "Statement_Standard_Abstract";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Publication_WebAddress" TO "Statement_Publication_WebAddress";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Publication_Urn" TO "Statement_Publication_Urn";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Publication_Title" TO "Statement_Publication_Title";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Publication_Section" TO "Statement_Publication_Section";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Publication_Doi" TO "Statement_Publication_Doi";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Publication_Authors" TO "Statement_Publication_Authors";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Publication_ArXiv" TO "Statement_Publication_ArXiv";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Publication_Abstract" TO "Statement_Publication_Abstract";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Year" TO "Statement_Standard_Year";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Title" TO "Statement_Standard_Title";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Standardizers" TO "Statement_Standard_Standardizers";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Section" TO "Statement_Standard_Section";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Numeration_Suffix" TO "Statement_Standard_Numeration_Suffix";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Numeration_Prefix" TO "Statement_Standard_Numeration_Prefix";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Numeration_MainNumber" TO "Statement_Standard_Numeration_MainNumber";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Locator" TO "Statement_Standard_Locator";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Standard_Abstract" TO "Statement_Standard_Abstract";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Publication_WebAddress" TO "Statement_Publication_WebAddress";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Publication_Urn" TO "Statement_Publication_Urn";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Publication_Title" TO "Statement_Publication_Title";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Publication_Section" TO "Statement_Publication_Section";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Publication_Doi" TO "Statement_Publication_Doi";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Publication_Authors" TO "Statement_Publication_Authors";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Publication_ArXiv" TO "Statement_Publication_ArXiv";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Publication_Abstract" TO "Statement_Publication_Abstract";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Year" TO "Statement_Standard_Year";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Title" TO "Statement_Standard_Title";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Standardizers" TO "Statement_Standard_Standardizers";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Section" TO "Statement_Standard_Section";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Numeration_Suffix" TO "Statement_Standard_Numeration_Suffix";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Numeration_Prefix" TO "Statement_Standard_Numeration_Prefix";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Numeration_MainNumber" TO "Statement_Standard_Numeration_MainNumber";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Locator" TO "Statement_Standard_Locator";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Standard_Abstract" TO "Statement_Standard_Abstract";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Publication_WebAddress" TO "Statement_Publication_WebAddress";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Publication_Urn" TO "Statement_Publication_Urn";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Publication_Title" TO "Statement_Publication_Title";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Publication_Section" TO "Statement_Publication_Section";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Publication_Doi" TO "Statement_Publication_Doi";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Publication_Authors" TO "Statement_Publication_Authors";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Publication_ArXiv" TO "Statement_Publication_ArXiv";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Publication_Abstract" TO "Statement_Publication_Abstract";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Year" TO "Statement_Standard_Year";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Title" TO "Statement_Standard_Title";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Standardizers" TO "Statement_Standard_Standardizers";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Section" TO "Statement_Standard_Section";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Numeration_Suffix" TO "Statement_Standard_Numeration_Suffix";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Numeration_Prefix" TO "Statement_Standard_Numeration_Prefix";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Numeration_MainNumber" TO "Statement_Standard_Numeration_MainNumber";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Locator" TO "Statement_Standard_Locator";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Standard_Abstract" TO "Statement_Standard_Abstract";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Publication_WebAddress" TO "Statement_Publication_WebAddress";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Publication_Urn" TO "Statement_Publication_Urn";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Publication_Title" TO "Statement_Publication_Title";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Publication_Section" TO "Statement_Publication_Section";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Publication_Doi" TO "Statement_Publication_Doi";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Publication_Authors" TO "Statement_Publication_Authors";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Publication_ArXiv" TO "Statement_Publication_ArXiv";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Publication_Abstract" TO "Statement_Publication_Abstract";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Year" TO "Statement_Standard_Year";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Title" TO "Statement_Standard_Title";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Standardizers" TO "Statement_Standard_Standardizers";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Section" TO "Statement_Standard_Section";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Numeration_Suffix" TO "Statement_Standard_Numeration_Suffix";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Numeration_Prefix" TO "Statement_Standard_Numeration_Prefix";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Numeration_MainNumber" TO "Statement_Standard_Numeration_MainNumber";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Locator" TO "Statement_Standard_Locator";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Standard_Abstract" TO "Statement_Standard_Abstract";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Publication_WebAddress" TO "Statement_Publication_WebAddress";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Publication_Urn" TO "Statement_Publication_Urn";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Publication_Title" TO "Statement_Publication_Title";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Publication_Section" TO "Statement_Publication_Section";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Publication_Doi" TO "Statement_Publication_Doi";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Publication_Authors" TO "Statement_Publication_Authors";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Publication_ArXiv" TO "Statement_Publication_ArXiv";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Publication_Abstract" TO "Statement_Publication_Abstract";

ALTER TABLE database."photovoltaic_data_Approvals" ADD "Statement_Exists" boolean NOT NULL DEFAULT TRUE;

ALTER TABLE database."optical_data_Approvals" ADD "Statement_Exists" boolean NOT NULL DEFAULT TRUE;

ALTER TABLE database."hygrothermal_data_Approvals" ADD "Statement_Exists" boolean NOT NULL DEFAULT TRUE;

ALTER TABLE database."geometric_data_Approvals" ADD "Statement_Exists" boolean NOT NULL DEFAULT TRUE;

ALTER TABLE database."calorimetric_data_Approvals" ADD "Statement_Exists" boolean NOT NULL DEFAULT TRUE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250530151605_ModelReferenceProperly', '9.0.5');

COMMIT;

