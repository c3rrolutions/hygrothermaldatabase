using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations;

/// <inheritdoc />
public partial class RenameResponsePropertyOfApprovalsToMessage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Response",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Message");

        migrationBuilder.RenameColumn(
            name: "Approval_Response",
            schema: "database",
            table: "photovoltaic_data",
            newName: "Approval_Message");

        migrationBuilder.RenameColumn(
            name: "Response",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Message");

        migrationBuilder.RenameColumn(
            name: "Approval_Response",
            schema: "database",
            table: "optical_data",
            newName: "Approval_Message");

        migrationBuilder.RenameColumn(
            name: "Response",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Message");

        migrationBuilder.RenameColumn(
            name: "Approval_Response",
            schema: "database",
            table: "hygrothermal_data",
            newName: "Approval_Message");

        migrationBuilder.RenameColumn(
            name: "Response",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Message");

        migrationBuilder.RenameColumn(
            name: "Approval_Response",
            schema: "database",
            table: "geometric_data",
            newName: "Approval_Message");

        migrationBuilder.RenameColumn(
            name: "Response",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Message");

        migrationBuilder.RenameColumn(
            name: "Approval_Response",
            schema: "database",
            table: "calorimetric_data",
            newName: "Approval_Message");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Message",
            schema: "database",
            table: "photovoltaic_data_Approvals",
            newName: "Response");

        migrationBuilder.RenameColumn(
            name: "Approval_Message",
            schema: "database",
            table: "photovoltaic_data",
            newName: "Approval_Response");

        migrationBuilder.RenameColumn(
            name: "Message",
            schema: "database",
            table: "optical_data_Approvals",
            newName: "Response");

        migrationBuilder.RenameColumn(
            name: "Approval_Message",
            schema: "database",
            table: "optical_data",
            newName: "Approval_Response");

        migrationBuilder.RenameColumn(
            name: "Message",
            schema: "database",
            table: "hygrothermal_data_Approvals",
            newName: "Response");

        migrationBuilder.RenameColumn(
            name: "Approval_Message",
            schema: "database",
            table: "hygrothermal_data",
            newName: "Approval_Response");

        migrationBuilder.RenameColumn(
            name: "Message",
            schema: "database",
            table: "geometric_data_Approvals",
            newName: "Response");

        migrationBuilder.RenameColumn(
            name: "Approval_Message",
            schema: "database",
            table: "geometric_data",
            newName: "Approval_Response");

        migrationBuilder.RenameColumn(
            name: "Message",
            schema: "database",
            table: "calorimetric_data_Approvals",
            newName: "Response");

        migrationBuilder.RenameColumn(
            name: "Approval_Message",
            schema: "database",
            table: "calorimetric_data",
            newName: "Approval_Response");
    }
}