using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.GeometricDataX;

public class QueryAllDataPayload
    : Payload
{
    public QueryAllDataPayload(
        ICollection<GeometricData> geometricData
    )
    {
        GeometricData = geometricData;
    }

    public QueryAllDataPayload(
        IReadOnlyCollection<IUserError> errors
    )
    {
        Errors = errors;
    }

    public QueryAllDataPayload(
        ICollection<GeometricData> geometricData,
        IReadOnlyCollection<IUserError> errors
    )
    {
        GeometricData = geometricData;
        Errors = errors;
    }

    public ICollection<GeometricData>? GeometricData { get; }
    public IReadOnlyCollection<IUserError>? Errors { get; }
}