using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.AccessRights;

[SuppressMessage("Naming", "CA1707")]
public enum AddAccessRightsErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    ALREADY_EXISTS,
    UNKNOWN_ACCESSRIGHTS
}