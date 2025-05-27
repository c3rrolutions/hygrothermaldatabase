namespace Database.GraphQl.AccessRights;

public sealed class AddInstitutionAccessRightsPayload
    : InstitutionAccessRightsPayload<InstitutionAccessRightsError>
{
    public AddInstitutionAccessRightsPayload(
        Data.InstitutionAccessRights accessRights
    )
        : base(accessRights)
    {
    }

    public AddInstitutionAccessRightsPayload(
        InstitutionAccessRightsError error
    )
        : base(error)
    {
    }
}