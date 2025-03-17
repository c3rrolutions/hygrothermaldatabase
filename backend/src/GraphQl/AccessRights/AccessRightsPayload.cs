using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class AccessRightsPayload<TAccessRightsError>
    : Payload
    where TAccessRightsError : IUserError
{
    protected AccessRightsPayload(
        Data.AccessRights accessRights
    )
    {
        AccessRights = accessRights;
    }

    protected AccessRightsPayload(
        IReadOnlyCollection<TAccessRightsError> errors
    )
    {
        Errors = errors;
    }

    protected AccessRightsPayload(
        TAccessRightsError error
    )
        : this(new[] { error })
    {
    }

    protected AccessRightsPayload(
        Data.AccessRights accessRights,
        IReadOnlyCollection<TAccessRightsError> errors
    )
    {
        AccessRights = accessRights;
        Errors = errors;
    }

    protected AccessRightsPayload(
        Data.AccessRights accessRights,
        TAccessRightsError error
    )
        : this(
            accessRights,
            new[] { error }
        )
    {
    }

    public Data.AccessRights? AccessRights { get; }
    public IReadOnlyCollection<TAccessRightsError>? Errors { get; }
}