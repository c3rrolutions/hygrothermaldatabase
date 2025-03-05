using Database.Data;

namespace Database.GraphQl.AccessRights;

public class AddDataAccessRightsPayload
    : AccessRightsPayload<AddDataAccessRightsError>
{
    public AddDataAccessRightsPayload(
        DataAccessRights dataAccessRights
    )
        : base(dataAccessRights)
    {
    }

    public AddDataAccessRightsPayload(
        AddDataAccessRightsError error
    )
        : base(error)
    {
    }
}