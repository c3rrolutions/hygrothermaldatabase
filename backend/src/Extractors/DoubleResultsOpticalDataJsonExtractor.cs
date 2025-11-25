using Json.Path;
using Database.Enumerations.DataPoints;

namespace Database.Extractors;

public sealed class DoubleResultsOpticalDataJsonExtractor(
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
        $")].results.{result.ToJsonProperty()}"
    ),
    ExtractNumberWithUncertainty
)
{
}