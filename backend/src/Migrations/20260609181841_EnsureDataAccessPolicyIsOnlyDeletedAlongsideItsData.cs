using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class EnsureDataAccessPolicyIsOnlyDeletedAlongsideItsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM database.data_access_policy;
                DELETE FROM database.user_access_policy;
                DELETE FROM database.institution_access_policy;
                DELETE FROM database.open_id_connect_application_access_policy;

                INSERT INTO database.data_access_policy ("CalorimetricDataId", "Combinator")
                    SELECT "Id", 'all'::database.logical_combinator
                    FROM database."calorimetric_data";

                INSERT INTO database.data_access_policy ("GeometricDataId", "Combinator")
                    SELECT "Id", 'all'::database.logical_combinator
                    FROM database."geometric_data";

                INSERT INTO database.data_access_policy ("HygrothermalDataId", "Combinator")
                    SELECT "Id", 'all'::database.logical_combinator
                    FROM database."hygrothermal_data";

                INSERT INTO database.data_access_policy ("LifeCycleDataId", "Combinator")
                    SELECT "Id", 'all'::database.logical_combinator
                    FROM database."lifeCycle_data";

                INSERT INTO database.data_access_policy ("OpticalDataId", "Combinator")
                    SELECT "Id", 'all'::database.logical_combinator
                    FROM database."optical_data";

                INSERT INTO database.data_access_policy ("PhotovoltaicDataId", "Combinator")
                    SELECT "Id", 'all'::database.logical_combinator
                    FROM database."photovoltaic_data";
            """);

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data\"() RETURNS trigger as $LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data$\r\nBEGIN\r\n  IF (\n    (OLD.\"CalorimetricDataId\" IS NOT NULL AND EXISTS (\n    SELECT 1 FROM database.\"calorimetric_data\" \n    WHERE \"Id\" = OLD.\"CalorimetricDataId\"\n)) OR (OLD.\"GeometricDataId\" IS NOT NULL AND EXISTS (\n    SELECT 1 FROM database.\"geometric_data\" \n    WHERE \"Id\" = OLD.\"GeometricDataId\"\n)) OR (OLD.\"HygrothermalDataId\" IS NOT NULL AND EXISTS (\n    SELECT 1 FROM database.\"hygrothermal_data\" \n    WHERE \"Id\" = OLD.\"HygrothermalDataId\"\n)) OR (OLD.\"LifeCycleDataId\" IS NOT NULL AND EXISTS (\n    SELECT 1 FROM database.\"lifeCycle_data\" \n    WHERE \"Id\" = OLD.\"LifeCycleDataId\"\n)) OR (OLD.\"OpticalDataId\" IS NOT NULL AND EXISTS (\n    SELECT 1 FROM database.\"optical_data\" \n    WHERE \"Id\" = OLD.\"OpticalDataId\"\n)) OR (OLD.\"PhotovoltaicDataId\" IS NOT NULL AND EXISTS (\n    SELECT 1 FROM database.\"photovoltaic_data\" \n    WHERE \"Id\" = OLD.\"PhotovoltaicDataId\"\n))\n) THEN\n    RAISE EXCEPTION 'You cannot delete a data access policy without also deleting the corresponding data.';\nEND IF;\r\nRETURN OLD;\r\nEND;\r\n$LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data BEFORE DELETE\r\nON \"database\".\"data_access_policy\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data\"();");

            migrationBuilder.Sql("CREATE FUNCTION \"database\".\"LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted\"() RETURNS trigger as $LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted$\r\nBEGIN\r\n  IF OLD.\"CalorimetricDataId\" IS NULL AND OLD.\"GeometricDataId\" IS NULL AND OLD.\"HygrothermalDataId\" IS NULL AND OLD.\"LifeCycleDataId\" IS NULL AND OLD.\"OpticalDataId\" IS NULL AND OLD.\"PhotovoltaicDataId\" IS NULL\nTHEN\n    RAISE EXCEPTION 'You cannot delete the global data access policy.';\nEND IF;\r\nRETURN OLD;\r\nEND;\r\n$LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted$ LANGUAGE plpgsql;\r\nCREATE TRIGGER LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted BEFORE DELETE\r\nON \"database\".\"data_access_policy\"\r\nFOR EACH ROW EXECUTE PROCEDURE \"database\".\"LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted\"();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_access_policy_can_only_be_deleted_after_data\"() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION \"database\".\"LC_TRIGGER_data_access_policy_global_policy_cannot_be_deleted\"() CASCADE;");

            migrationBuilder.Sql("""
                DELETE FROM database.data_access_policy;
                DELETE FROM database.user_access_policy;
                DELETE FROM database.institution_access_policy;
                DELETE FROM database.open_id_connect_application_access_policy;
            """);
        }
    }
}