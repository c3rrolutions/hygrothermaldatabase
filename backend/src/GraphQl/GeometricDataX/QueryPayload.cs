using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.GeometricDataX;

public class QueryPayload
    : Payload
{
    public QueryPayload(
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