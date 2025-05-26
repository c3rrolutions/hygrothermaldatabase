using Database.Data;

namespace Database.GraphQl.OpticalDataX;

public sealed class CreateOpticalDataPayload
    : OpticalDataPayload<CreateOpticalDataError>
{
    public CreateOpticalDataPayload(
        OpticalData opticalData
    )
        : base(opticalData)
    {
    }

    public CreateOpticalDataPayload(
        CreateOpticalDataError error
    )
        : base(error)
    {
    }

    public CreateOpticalDataPayload(
        OpticalData opticalData,
        CreateOpticalDataError error
    )
        : base(opticalData, error)
    {
    }
}