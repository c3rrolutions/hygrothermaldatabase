namespace Database.GraphQl.AccessRights;

public class AddAccessRightsPayload
    : AccessRightsPayload<AddAccessRightsError>
{
    public AddAccessRightsPayload(
        Data.AccessRights accessRights
    )
        : base(accessRights)
    {
    }

    public AddAccessRightsPayload(
        AddAccessRightsError error
    )
        : base(error)
    {
    }
}