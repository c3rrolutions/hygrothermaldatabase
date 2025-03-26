using System.Diagnostics.CodeAnalysis;

namespace Database.GraphQl.AccessRights;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateDataAccessRightsErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}