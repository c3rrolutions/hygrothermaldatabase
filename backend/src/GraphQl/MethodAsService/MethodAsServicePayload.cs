using System.Collections.Generic;

namespace Database.GraphQl.MethodAsService;

public class MethodAsServicePayload
    : Payload
{
    public MethodAsServicePayload(
        IReadOnlyList<DataPoint> calculationResult
    )
    {
        CalculationResult = calculationResult;
    }

    public MethodAsServicePayload(
        IReadOnlyCollection<MethodAsServiceError> errors
    )
    {
        Errors = errors;
    }

    public MethodAsServicePayload(
        MethodAsServiceError error
    )
        : this(new[] { error })
    {
    }

    public MethodAsServicePayload(
        IReadOnlyList<DataPoint> calculationResult,
        IReadOnlyCollection<MethodAsServiceError> errors
    )
    {
        CalculationResult = calculationResult;
        Errors = errors;
    }

    public MethodAsServicePayload(
        List<DataPoint> calculationResult,
        MethodAsServiceError error
    )
        : this(
            calculationResult,
            new[] { error }
        )
    {
    }

    public IReadOnlyList<DataPoint>? CalculationResult { get; }
    public IReadOnlyCollection<MethodAsServiceError>? Errors { get; }
}