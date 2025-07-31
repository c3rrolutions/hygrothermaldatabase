using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.DataApprovals;

[SuppressMessage("Naming", "CA1707")]
public enum AddDataApprovalErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_DATABASE,
    CREATING_RESPONSE_APPROVAL_FAILED,
    AMBIGUOUS_STATEMENT,
    MISSING_STATEMENT
}