using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.PhotovoltaicDataX;

public abstract class PhotovoltaicDataPayload<TPhotovoltaicDataError>
    : Payload
    where TPhotovoltaicDataError : IUserError
{
    protected PhotovoltaicDataPayload(
        PhotovoltaicData photovoltaicData
    )
    {
        PhotovoltaicData = photovoltaicData;
    }

    protected PhotovoltaicDataPayload(
        IReadOnlyCollection<TPhotovoltaicDataError> errors
    )
    {
        Errors = errors;
    }

    protected PhotovoltaicDataPayload(
        TPhotovoltaicDataError error
    )
        : this([error])
    {
    }

    protected PhotovoltaicDataPayload(
        PhotovoltaicData photovoltaicData,
        IReadOnlyCollection<TPhotovoltaicDataError> errors
    )
    {
        PhotovoltaicData = photovoltaicData;
        Errors = errors;
    }

    protected PhotovoltaicDataPayload(
        PhotovoltaicData photovoltaicData,
        TPhotovoltaicDataError error
    )
        : this(
            photovoltaicData,
            [error]
        )
    {
    }

    public PhotovoltaicData? PhotovoltaicData { get; }
    public IReadOnlyCollection<TPhotovoltaicDataError>? Errors { get; }
}