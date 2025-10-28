namespace Database.GraphQl.Numerations;

public sealed record NumerationInput(
    string? Prefix,
    string MainNumber,
    string? Suffix
);