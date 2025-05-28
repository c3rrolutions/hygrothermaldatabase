using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.CalorimetricDataX;

public abstract class CalorimetricDataPayload<TCalorimetricDataError>
    : Payload
    where TCalorimetricDataError : IUserError
{
    protected CalorimetricDataPayload(
        CalorimetricData calorimetricData
    )
    {
        CalorimetricData = calorimetricData;
    }

    protected CalorimetricDataPayload(
        IReadOnlyCollection<TCalorimetricDataError> errors
    )
    {
        Errors = errors;
    }

    protected CalorimetricDataPayload(
        TCalorimetricDataError error
    )
        : this([error])
    {
    }

    protected CalorimetricDataPayload(
        CalorimetricData calorimetricData,
        IReadOnlyCollection<TCalorimetricDataError> errors
    )
    {
        CalorimetricData = calorimetricData;
        Errors = errors;
    }

    protected CalorimetricDataPayload(
        CalorimetricData calorimetricData,
        TCalorimetricDataError error
    )
        : this(
            calorimetricData,
            [error]
        )
    {
    }

    public CalorimetricData? CalorimetricData { get; }
    public IReadOnlyCollection<TCalorimetricDataError>? Errors { get; }
}