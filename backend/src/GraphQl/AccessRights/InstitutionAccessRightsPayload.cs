using System.Collections.Generic;

namespace Database.GraphQl.AccessRights;

public class InstitutionAccessRightsPayload<TInstitutionAccessRightsError>
    : Payload
    where TInstitutionAccessRightsError : IUserError
{
    protected InstitutionAccessRightsPayload(
        Data.InstitutionAccessRights accessRights
    )
    {
        AccessRights = accessRights;
    }

    protected InstitutionAccessRightsPayload(
        IReadOnlyCollection<TInstitutionAccessRightsError> errors
    )
    {
        Errors = errors;
    }

    protected InstitutionAccessRightsPayload(
        TInstitutionAccessRightsError error
    )
        : this(new[] { error })
    {
    }

    protected InstitutionAccessRightsPayload(
        Data.InstitutionAccessRights accessRights,
        IReadOnlyCollection<TInstitutionAccessRightsError> errors
    )
    {
        AccessRights = accessRights;
        Errors = errors;
    }

    protected InstitutionAccessRightsPayload(
        Data.InstitutionAccessRights accessRights,
        TInstitutionAccessRightsError error
    )
        : this(
            accessRights,
            new[] { error }
        )
    {
    }

    public Data.InstitutionAccessRights? AccessRights { get; }
    public IReadOnlyCollection<TInstitutionAccessRightsError>? Errors { get; }
}