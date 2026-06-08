START TRANSACTION;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data"() CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData') THEN
    DROP FUNCTION "database"."LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted"() CASCADE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData') THEN
        DELETE FROM database.data_access_policy;
        DELETE FROM database.user_access_policy;
        DELETE FROM database.institution_access_policy;
        DELETE FROM database.open_id_connect_application_access_policy;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20260609181841_EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData';
    END IF;
END $EF$;

COMMIT;

