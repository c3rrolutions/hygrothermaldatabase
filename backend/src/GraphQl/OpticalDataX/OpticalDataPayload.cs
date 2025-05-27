using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.OpticalDataX;

public abstract class OpticalDataPayload<TOpticalDataError>
    : Payload
    where TOpticalDataError : IUserError
{
    protected OpticalDataPayload(
        OpticalData opticalData
    )
    {
        OpticalData = opticalData;
    }

    protected OpticalDataPayload(
        IReadOnlyCollection<TOpticalDataError> errors
    )
    {
        Errors = errors;
    }

    protected OpticalDataPayload(
        TOpticalDataError error
    )
        : this([error])
    {
    }

    protected OpticalDataPayload(
        OpticalData opticalData,
        IReadOnlyCollection<TOpticalDataError> errors
    )
    {
        OpticalData = opticalData;
        Errors = errors;
    }

    protected OpticalDataPayload(
        OpticalData opticalData,
        TOpticalDataError error
    )
        : this(
            opticalData,
            [error]
        )
    {
    }

    public OpticalData? OpticalData { get; }
    public IReadOnlyCollection<TOpticalDataError>? Errors { get; }
}