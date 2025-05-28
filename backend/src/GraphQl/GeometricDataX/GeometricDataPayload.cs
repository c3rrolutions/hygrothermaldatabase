using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.GeometricDataX;

public abstract class GeometricDataPayload<TGeometricDataError>
    : Payload
    where TGeometricDataError : IUserError
{
    protected GeometricDataPayload(
        GeometricData geometricData
    )
    {
        GeometricData = geometricData;
    }

    protected GeometricDataPayload(
        IReadOnlyCollection<TGeometricDataError> errors
    )
    {
        Errors = errors;
    }

    protected GeometricDataPayload(
        TGeometricDataError error
    )
        : this([error])
    {
    }

    protected GeometricDataPayload(
        GeometricData geometricData,
        IReadOnlyCollection<TGeometricDataError> errors
    )
    {
        GeometricData = geometricData;
        Errors = errors;
    }

    protected GeometricDataPayload(
        GeometricData geometricData,
        TGeometricDataError error
    )
        : this(
            geometricData,
            [error]
        )
    {
    }

    public GeometricData? GeometricData { get; }
    public IReadOnlyCollection<TGeometricDataError>? Errors { get; }
}