START TRANSACTION;
ALTER TABLE database."photovoltaic_data_Approvals" DROP COLUMN "Statement_Exists";

ALTER TABLE database."optical_data_Approvals" DROP COLUMN "Statement_Exists";

ALTER TABLE database."hygrothermal_data_Approvals" DROP COLUMN "Statement_Exists";

ALTER TABLE database."geometric_data_Approvals" DROP COLUMN "Statement_Exists";

ALTER TABLE database."calorimetric_data_Approvals" DROP COLUMN "Statement_Exists";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Year" TO "Standard_Year";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Title" TO "Standard_Title";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Standardizers" TO "Standard_Standardizers";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Section" TO "Standard_Section";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Suffix" TO "Standard_Numeration_Suffix";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Prefix" TO "Standard_Numeration_Prefix";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_MainNumber" TO "Standard_Numeration_MainNumber";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Locator" TO "Standard_Locator";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Standard_Abstract" TO "Standard_Abstract";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Publication_WebAddress" TO "Publication_WebAddress";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Publication_Urn" TO "Publication_Urn";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Publication_Title" TO "Publication_Title";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Publication_Section" TO "Publication_Section";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Publication_Doi" TO "Publication_Doi";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Publication_Authors" TO "Publication_Authors";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Publication_ArXiv" TO "Publication_ArXiv";

ALTER TABLE database."photovoltaic_data_Approvals" RENAME COLUMN "Statement_Publication_Abstract" TO "Publication_Abstract";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Year" TO "Standard_Year";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Title" TO "Standard_Title";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Standardizers" TO "Standard_Standardizers";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Section" TO "Standard_Section";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Suffix" TO "Standard_Numeration_Suffix";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Prefix" TO "Standard_Numeration_Prefix";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_MainNumber" TO "Standard_Numeration_MainNumber";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Locator" TO "Standard_Locator";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Standard_Abstract" TO "Standard_Abstract";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Publication_WebAddress" TO "Publication_WebAddress";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Publication_Urn" TO "Publication_Urn";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Publication_Title" TO "Publication_Title";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Publication_Section" TO "Publication_Section";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Publication_Doi" TO "Publication_Doi";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Publication_Authors" TO "Publication_Authors";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Publication_ArXiv" TO "Publication_ArXiv";

ALTER TABLE database."optical_data_Approvals" RENAME COLUMN "Statement_Publication_Abstract" TO "Publication_Abstract";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Year" TO "Standard_Year";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Title" TO "Standard_Title";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Standardizers" TO "Standard_Standardizers";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Section" TO "Standard_Section";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Suffix" TO "Standard_Numeration_Suffix";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Prefix" TO "Standard_Numeration_Prefix";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_MainNumber" TO "Standard_Numeration_MainNumber";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Locator" TO "Standard_Locator";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Standard_Abstract" TO "Standard_Abstract";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Publication_WebAddress" TO "Publication_WebAddress";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Publication_Urn" TO "Publication_Urn";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Publication_Title" TO "Publication_Title";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Publication_Section" TO "Publication_Section";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Publication_Doi" TO "Publication_Doi";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Publication_Authors" TO "Publication_Authors";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Publication_ArXiv" TO "Publication_ArXiv";

ALTER TABLE database."hygrothermal_data_Approvals" RENAME COLUMN "Statement_Publication_Abstract" TO "Publication_Abstract";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Year" TO "Standard_Year";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Title" TO "Standard_Title";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Standardizers" TO "Standard_Standardizers";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Section" TO "Standard_Section";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Suffix" TO "Standard_Numeration_Suffix";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Prefix" TO "Standard_Numeration_Prefix";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_MainNumber" TO "Standard_Numeration_MainNumber";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Locator" TO "Standard_Locator";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Standard_Abstract" TO "Standard_Abstract";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Publication_WebAddress" TO "Publication_WebAddress";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Publication_Urn" TO "Publication_Urn";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Publication_Title" TO "Publication_Title";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Publication_Section" TO "Publication_Section";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Publication_Doi" TO "Publication_Doi";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Publication_Authors" TO "Publication_Authors";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Publication_ArXiv" TO "Publication_ArXiv";

ALTER TABLE database."geometric_data_Approvals" RENAME COLUMN "Statement_Publication_Abstract" TO "Publication_Abstract";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Year" TO "Standard_Year";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Title" TO "Standard_Title";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Standardizers" TO "Standard_Standardizers";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Section" TO "Standard_Section";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Suffix" TO "Standard_Numeration_Suffix";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_Prefix" TO "Standard_Numeration_Prefix";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Numeration_MainNumber" TO "Standard_Numeration_MainNumber";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Locator" TO "Standard_Locator";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Standard_Abstract" TO "Standard_Abstract";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Publication_WebAddress" TO "Publication_WebAddress";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Publication_Urn" TO "Publication_Urn";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Publication_Title" TO "Publication_Title";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Publication_Section" TO "Publication_Section";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Publication_Doi" TO "Publication_Doi";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Publication_Authors" TO "Publication_Authors";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Publication_ArXiv" TO "Publication_ArXiv";

ALTER TABLE database."calorimetric_data_Approvals" RENAME COLUMN "Statement_Publication_Abstract" TO "Publication_Abstract";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250530151605_ModelReferenceProperly';

ALTER TABLE database.institution_access_rights DROP CONSTRAINT "PK_institution_access_rights";

ALTER TABLE database.institution_access_rights RENAME TO access_rights;

ALTER TABLE database.access_rights ADD CONSTRAINT "PK_access_rights" PRIMARY KEY ("Id");

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250527150325_MoveInstitutionAccessRights';

ALTER TABLE database.photovoltaic_data DROP COLUMN "DataAccessRights_AllowedApplications";

ALTER TABLE database.photovoltaic_data DROP COLUMN "DataAccessRights_AllowedInstitutions";

ALTER TABLE database.photovoltaic_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";

ALTER TABLE database.optical_data DROP COLUMN "DataAccessRights_AllowedApplications";

ALTER TABLE database.optical_data DROP COLUMN "DataAccessRights_AllowedInstitutions";

ALTER TABLE database.optical_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";

ALTER TABLE database.hygrothermal_data DROP COLUMN "DataAccessRights_AllowedApplications";

ALTER TABLE database.hygrothermal_data DROP COLUMN "DataAccessRights_AllowedInstitutions";

ALTER TABLE database.hygrothermal_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";

ALTER TABLE database.geometric_data DROP COLUMN "DataAccessRights_AllowedApplications";

ALTER TABLE database.geometric_data DROP COLUMN "DataAccessRights_AllowedInstitutions";

ALTER TABLE database.geometric_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";

ALTER TABLE database.calorimetric_data DROP COLUMN "DataAccessRights_AllowedApplications";

ALTER TABLE database.calorimetric_data DROP COLUMN "DataAccessRights_AllowedInstitutions";

ALTER TABLE database.calorimetric_data DROP COLUMN "DataAccessRights_AllowedUserAndQuantity";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250527144412_AddDataAccessRights';

COMMIT;

