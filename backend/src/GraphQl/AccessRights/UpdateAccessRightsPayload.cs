namespace Database.GraphQl.AccessRights;

public class UpdateAccessRightsPayload
    : AccessRightsPayload<AccessRightsError>
{
    public UpdateAccessRightsPayload(
        Data.AccessRights accessRights
    )
        : base(accessRights)
    {
    }

    public UpdateAccessRightsPayload(
        AccessRightsError error
    )
        : base(error)
    {
    }
}