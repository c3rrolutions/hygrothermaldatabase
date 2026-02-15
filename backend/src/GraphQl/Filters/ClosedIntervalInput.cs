namespace Database.GraphQl.Filters;

public sealed record ClosedIntervalInput(
    double LowerBound,
    double UpperBound
);