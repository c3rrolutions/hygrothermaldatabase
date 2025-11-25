using System;
using System.Diagnostics.CodeAnalysis;
using Json.Path;

namespace Database.Extractors;

[SuppressMessage("Naming", "CA1707")]
public enum CalorimetricResult
{
    G_VALUE, // Solar Gain
    U_VALUE // CalorimetricResult
}

public static class CalorimetricResultExtensions
{
    public static string ToJsonProperty(this CalorimetricResult result)
    {
        return result switch
        {
            CalorimetricResult.U_VALUE => "uValue",
            CalorimetricResult.G_VALUE => "gValue",
            _ => throw new ArgumentOutOfRangeException(nameof(result), $"Unsupported result {result}."),
        };
    }
}

public sealed class DoubleResultsCalorimetricDataJsonExtractor(
    CalorimetricResult result
) : StructJsonPathExtractor<double>(
    JsonPath.Parse(
        $"$.results[*].{result.ToJsonProperty()}"
    ),
    ExtractNumberWithUncertainty
)
{
}