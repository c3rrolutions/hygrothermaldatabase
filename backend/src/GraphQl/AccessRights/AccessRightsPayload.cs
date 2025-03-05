using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.AccessRights;

public class AccessRightsPayload<TDataAccessRightsError>
    : Payload
    where TDataAccessRightsError : IUserError
{
    protected AccessRightsPayload(
        DataAccessRights dataAccessRights
    )
    {
        DataAccessRights = dataAccessRights;
    }

    protected AccessRightsPayload(
        IReadOnlyCollection<TDataAccessRightsError> errors
    )
    {
        Errors = errors;
    }

    protected AccessRightsPayload(
        TDataAccessRightsError error
    )
        : this(new[] { error })
    {
    }

    protected AccessRightsPayload(
        DataAccessRights dataAccessRights,
        IReadOnlyCollection<TDataAccessRightsError> errors
    )
    {
        DataAccessRights = dataAccessRights;
        Errors = errors;
    }

    protected AccessRightsPayload(
        DataAccessRights dataAccessRights,
        TDataAccessRightsError error
    )
        : this(
            dataAccessRights,
            new[] { error }
        )
    {
    }

    public DataAccessRights? DataAccessRights { get; }
    public IReadOnlyCollection<TDataAccessRightsError>? Errors { get; }
}