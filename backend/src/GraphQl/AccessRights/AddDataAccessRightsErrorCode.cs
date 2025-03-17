using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.AccessRights;

[SuppressMessage("Naming", "CA1707")]
public enum AddDataAccessRightsErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}