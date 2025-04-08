namespace Database.GraphQl.AccessRights;

public class UpdateAccessRightsPayload
    : InstitutionAccessRightsPayload<InstitutionAccessRightsError>
{
    public UpdateAccessRightsPayload(
        Data.InstitutionAccessRights accessRights
    )
        : base(accessRights)
    {
    }

    public UpdateAccessRightsPayload(
        InstitutionAccessRightsError error
    )
        : base(error)
    {
    }
}