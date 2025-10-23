using Json.Path;
using Database.Enumerations.DataPoints;

namespace Database.Extractors;

public sealed class ColorRenderingIndexResultsJsonExtractor(
) : StructJsonPathExtractor<double>(
    JsonPath.Parse(
        $"$.data[*].dataPoints[?(" +
        $"@.incidence.wavelengths.integral=='{WavelengthsIntegral.VISIBLE.ToJsonEnum()}'" +
        $")].results.colorTransmittance.colorRenderingIndices[*]['ra','raOutIn']"
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