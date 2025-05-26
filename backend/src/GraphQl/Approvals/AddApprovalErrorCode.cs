using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.Approvals;

[SuppressMessage("Naming", "CA1707")]
public enum AddApprovalErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    SIGNING_FAILED
}