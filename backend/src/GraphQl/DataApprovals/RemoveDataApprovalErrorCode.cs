using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.DataApprovals;

[SuppressMessage("Naming", "CA1707")]
public enum RemoveDataApprovalErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_APPROVAL,
    CREATING_RESPONSE_APPROVAL_FAILED
}