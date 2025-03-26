using Database.Data;

namespace Database.GraphQl.GeometricDataX;

public class QueryDataPayload
    : Payload
{
    public QueryDataPayload(
        GeometricData? geometricData
    )
    {
        GeometricData = geometricData;
    }

    public QueryDataPayload(
        IUserError errors
    )
    {
        Errors = errors;
    }

    public QueryDataPayload(
        GeometricData? geometricData,
        IUserError errors
    )
    {
        GeometricData = geometricData;
        Errors = errors;
    }

    public GeometricData? GeometricData { get; }
    public IUserError? Errors { get; }
}