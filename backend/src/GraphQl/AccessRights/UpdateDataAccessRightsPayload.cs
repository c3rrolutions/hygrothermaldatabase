using Database.Data;

namespace Database.GraphQl.AccessRights;

public sealed class UpdateDataAccessRightsPayload
    : DataAccessRightsPayload<UpdateDataAccessRightsError>
{
    public UpdateDataAccessRightsPayload(
        DataAccessRights dataAccessRights
    )
        : base(dataAccessRights)
    {
    }

    public UpdateDataAccessRightsPayload(
        UpdateDataAccessRightsError error
    )
        : base(error)
    {
    }
}