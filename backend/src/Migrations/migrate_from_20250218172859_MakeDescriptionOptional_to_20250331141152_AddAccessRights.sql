START TRANSACTION;
ALTER TABLE database.photovoltaic_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.photovoltaic_data ADD "Approval_Query" text;

ALTER TABLE database.photovoltaic_data ADD "Approval_Response" text;

ALTER TABLE database.photovoltaic_data ADD "Approval_Signature" text;

ALTER TABLE database.photovoltaic_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.optical_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.optical_data ADD "Approval_Query" text;

ALTER TABLE database.optical_data ADD "Approval_Response" text;

ALTER TABLE database.optical_data ADD "Approval_Signature" text;

ALTER TABLE database.optical_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.hygrothermal_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.hygrothermal_data ADD "Approval_Query" text;

ALTER TABLE database.hygrothermal_data ADD "Approval_Response" text;

ALTER TABLE database.hygrothermal_data ADD "Approval_Signature" text;

ALTER TABLE database.hygrothermal_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.geometric_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.geometric_data ADD "Approval_Query" text;

ALTER TABLE database.geometric_data ADD "Approval_Response" text;

ALTER TABLE database.geometric_data ADD "Approval_Signature" text;

ALTER TABLE database.geometric_data ADD "Approval_Timestamp" timestamp with time zone;

ALTER TABLE database.calorimetric_data ADD "Approval_KeyFingerprint" text;

ALTER TABLE database.calorimetric_data ADD "Approval_Query" text;

ALTER TABLE database.calorimetric_data ADD "Approval_Response" text;

ALTER TABLE database.calorimetric_data ADD "Approval_Signature" text;

ALTER TABLE database.calorimetric_data ADD "Approval_Timestamp" timestamp with time zone;

CREATE TABLE database.access_rights (
    "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
    "InstitutionId" uuid NOT NULL,
    "AllowedUserCount" bigint,
    "AllowedDatasetsPerTime" bigint,
    "Period" interval NOT NULL,
    "UserAlreadyAccessed" uuid[] NOT NULL,
    CONSTRAINT "PK_access_rights" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250331141152_AddAccessRights', '9.0.2');

COMMIT;

