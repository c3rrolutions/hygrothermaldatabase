using System.Diagnostics.CodeAnalysis;

namespace Database.Enumerations;

[SuppressMessage("Naming", "CA1707")]
public enum DataKind
{
    CALORIMETRIC_DATA,
    GEOMETRIC_DATA,
    HYGROTHERMAL_DATA,
    LIFE_CYCLE_DATA,
    OPTICAL_DATA,
    PHOTOVOLTAIC_DATA
}