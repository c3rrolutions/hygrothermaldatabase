using System;
using System.Diagnostics.CodeAnalysis;
using Json.Path;

namespace Database.Extractors;

[SuppressMessage("Naming", "CA1707")]
public enum Dimension
{
    WIDTH,
    HEIGHT,
    THICKNESS
}

public static class DimensionExtensions
{
    public static string ToJsonProperty(this Dimension dimension)
    {
        return dimension switch
        {
            Dimension.WIDTH => "width",
            Dimension.HEIGHT => "height",
            Dimension.THICKNESS => "thickness",
            _ => throw new ArgumentOutOfRangeException(nameof(dimension), $"Unsupported dimension {dimension}."),
        };
    }
}

public sealed class InstalledDimensionsGeometricDataJsonExtractor(
    Dimension dimension
) : StructJsonPathExtractor<double>(
    JsonPath.Parse(
        $"$.dimensions.installed[*].{dimension.ToJsonProperty()}"
    ),
    ExtractNumberWithUncertainty
)
{
}