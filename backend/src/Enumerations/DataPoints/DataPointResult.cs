using System.Diagnostics.CodeAnalysis;

namespace Database.Enumerations.DataPoints;

[SuppressMessage("Naming", "CA1707")]
public enum DataPointResult
{
    TRANSMITTANCE,
    REFLECTANCE,
    ABSORPTANCE_EMITTANCE
}