using Database.Enumerations.DataPoints;
using Json.Path;

namespace Database.Extractors;

public sealed class ColorRenderingIndexResultsJsonExtractor(
) : StructJsonPathExtractor<double>(
    JsonPath.Parse(
        $"$.data[*].dataPoints[?(" +
        $"@.incidence.wavelengths.integral=='{WavelengthsIntegral.VISIBLE.ToJsonEnum()}'" +
        $")].results.colorTransmittance.colorRenderingIndices[*]['ra','raOutIn']"
    ),
    ExtractNumberWithUncertainty
)
{
}