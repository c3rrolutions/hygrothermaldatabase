using Database.Data;

namespace Database.GraphQl.HygrothermalDataX;

public sealed class CreateHygrothermalDataPayload
    : HygrothermalDataPayload<CreateHygrothermalDataError>
{
    public CreateHygrothermalDataPayload(
        HygrothermalData hygrothermalData
    )
        : base(hygrothermalData)
    {
    }

    public CreateHygrothermalDataPayload(
        CreateHygrothermalDataError error
    )
        : base(error)
    {
    }

    public CreateHygrothermalDataPayload(
        HygrothermalData hygrothermalData,
        CreateHygrothermalDataError error
    )
        : base(hygrothermalData, error)
    {
    }
}