using System.Text.Json;
using Database.Data;
using Database.Enumerations.DataPoints;
using Database.Json;
using Json.Path;

namespace Database.Extractors;

public sealed class CielabColorResultsJsonExtractor(
) : ClassJsonPathExtractor<CielabColor>(
    JsonPath.Parse(
        $"$.data[*].dataPoints[?(" +
        $"@.incidence.wavelengths.integral=='{WavelengthsIntegral.VISIBLE.ToJsonEnum()}'" +
        ")].results['colorTransmittance','colorReflectance'].cielabColors[*]"
    ),
    node => node.Deserialize<CielabColor>(JsonSerializerSettings.BedJson)
)
{
}