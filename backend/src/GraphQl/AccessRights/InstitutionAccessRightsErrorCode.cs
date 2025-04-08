using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.AccessRights;

[SuppressMessage("Naming", "CA1707")]
public enum InstitutionAccessRightsErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    ALREADY_EXISTS,
    UNKNOWN_ACCESS_RIGHTS
}