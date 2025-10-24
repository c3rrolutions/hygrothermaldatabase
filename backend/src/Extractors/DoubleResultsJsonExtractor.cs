using Json.Path;
using Database.Enumerations.DataPoints;

namespace Database.Extractors;

public sealed class DoubleResultsJsonExtractor(
    IncidenceDirection incidenceDirection,
    WavelengthsIntegral incidenceWavelengthsIntegral,
    EmergenceDirection emergenceDirection,
    DataPointResult result
) : StructJsonPathExtractor<double>(
    JsonPath.Parse(
        $"$.data[*].dataPoints[?(" +
        $"@.incidence.direction{incidenceDirection.ToJsonPathQuery()} && " +
        $"@.incidence.wavelengths.integral=='{incidenceWavelengthsIntegral.ToJsonEnum()}' && " +
        $"@.emergence.direction=='{emergenceDirection.ToJsonEnum()}'" +
        $")].results.{result.ToJsonEnum()}"
    ),
    node =>
    {
        if (node.TryGetValue<double>(out var value))
        {
            return value;
        }
        return null;
    }
)
{
}