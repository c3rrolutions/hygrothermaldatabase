using Database.Data;
using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class DataAccessRightsPayload<TDataAccessRightsError>
    : Payload
    where TDataAccessRightsError : IUserError
{
    protected DataAccessRightsPayload(
        DataAccessRights dataAccessRights
    )
    {
        DataAccessRights = dataAccessRights;
    }

    protected DataAccessRightsPayload(
        IReadOnlyCollection<TDataAccessRightsError> errors
    )
    {
        Errors = errors;
    }

    protected DataAccessRightsPayload(
        TDataAccessRightsError error
    )
        : this(new[] { error })
    {
    }

    protected DataAccessRightsPayload(
        DataAccessRights dataAccessRights,
        IReadOnlyCollection<TDataAccessRightsError> errors
    )
    {
        DataAccessRights = dataAccessRights;
        Errors = errors;
    }

    protected DataAccessRightsPayload(
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