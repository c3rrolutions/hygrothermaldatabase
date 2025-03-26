START TRANSACTION;
ALTER TABLE database.photovoltaic_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.photovoltaic_data ADD "Approval_Query" text;

ALTER TABLE database.photovoltaic_data ADD "Approval_Response" text;

ALTER TABLE database.photovoltaic_data ADD "Approval_Signature" text;

ALTER TABLE database.photovoltaic_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.photovoltaic_data ADD "DataAccess" integer NOT NULL DEFAULT 0;

ALTER TABLE database.optical_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.optical_data ADD "Approval_Query" text;

ALTER TABLE database.optical_data ADD "Approval_Response" text;

ALTER TABLE database.optical_data ADD "Approval_Signature" text;

ALTER TABLE database.optical_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.optical_data ADD "DataAccess" integer NOT NULL DEFAULT 0;

ALTER TABLE database.hygrothermal_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.hygrothermal_data ADD "Approval_Query" text;

ALTER TABLE database.hygrothermal_data ADD "Approval_Response" text;

ALTER TABLE database.hygrothermal_data ADD "Approval_Signature" text;

ALTER TABLE database.hygrothermal_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.hygrothermal_data ADD "DataAccess" integer NOT NULL DEFAULT 0;

ALTER TABLE database.get_https_resource ALTER COLUMN "Description" DROP NOT NULL;

ALTER TABLE database.geometric_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.geometric_data ADD "Approval_Query" text;

ALTER TABLE database.geometric_data ADD "Approval_Response" text;

ALTER TABLE database.geometric_data ADD "Approval_Signature" text;

ALTER TABLE database.geometric_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.geometric_data ADD "DataAccess" integer NOT NULL DEFAULT 0;

ALTER TABLE database.calorimetric_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.calorimetric_data ADD "Approval_Query" text;

ALTER TABLE database.calorimetric_data ADD "Approval_Response" text;

ALTER TABLE database.calorimetric_data ADD "Approval_Signature" text;

ALTER TABLE database.calorimetric_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.calorimetric_data ADD "DataAccess" integer NOT NULL DEFAULT 0;

CREATE TABLE database.access_rights (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "InstitutionId" uuid NOT NULL,
    "AllowedUserCount" integer NOT NULL,
    "UserOfInstitution" uuid[] NOT NULL,
    "AllowedDatasetsPerTime" integer NOT NULL,
    "Period" interval NOT NULL,
    CONSTRAINT "PK_access_rights" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250319132121_AddAccessRights', '9.0.2');

COMMIT;

