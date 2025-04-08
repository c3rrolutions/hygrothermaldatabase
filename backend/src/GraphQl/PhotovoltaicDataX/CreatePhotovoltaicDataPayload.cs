using Database.Data;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed class CreatePhotovoltaicDataPayload
    : PhotovoltaicDataPayload<CreatePhotovoltaicDataError>
{
    public CreatePhotovoltaicDataPayload(
        PhotovoltaicData photovoltaicData
    )
        : base(photovoltaicData)
    {
    }

    public CreatePhotovoltaicDataPayload(
        CreatePhotovoltaicDataError error
    )
        : base(error)
    {
    }

    public CreatePhotovoltaicDataPayload(
        PhotovoltaicData photovoltaicData,
        CreatePhotovoltaicDataError error
    )
        : base(photovoltaicData, error)
    {
    }
}