using System;

namespace Database.Enumerations.DataPoints;

public static class DataPointsEnumerationsExtensions
{
    public static string ToJsonPathQuery(this IncidenceDirection incidenceDirection)
    {
        return incidenceDirection switch
        {
            IncidenceDirection.NEARNORMAL => ".polar <= 10",
            IncidenceDirection.HEMISPHERICAL => "=='hemispherical'",
            _ => throw new ArgumentOutOfRangeException(nameof(incidenceDirection), $"Unsupported incidence direction {incidenceDirection}."),
        };
    }

    public static string ToJsonEnum(this WavelengthsIntegral wavelengthsIntegral)
    {
        return wavelengthsIntegral switch
        {
            WavelengthsIntegral.INFRARED => "infrared",
            WavelengthsIntegral.SOLAR => "solar",
            WavelengthsIntegral.ULTRAVIOLET => "ultraviolet",
            WavelengthsIntegral.VISIBLE => "visible",
            WavelengthsIntegral.OTHER => "other",
            _ => throw new ArgumentOutOfRangeException(nameof(wavelengthsIntegral), $"Unsupported wavelengths integral {wavelengthsIntegral}."),
        };
    }

    public static string ToJsonEnum(this EmergenceDirection emergenceDirection)
    {
        return emergenceDirection switch
        {
            EmergenceDirection.DIFFUSE => "diffuse",
            EmergenceDirection.HEMISPHERICAL => "hemispherical",
            _ => throw new ArgumentOutOfRangeException(nameof(emergenceDirection), $"Unsupported emergence direction {emergenceDirection}."),
        };
    }

    public static string ToJsonProperty(this DataPointResult result)
    {
        return result switch
        {
            DataPointResult.TRANSMITTANCE => "transmittance",
            DataPointResult.REFLECTANCE => "reflectance",
            DataPointResult.ABSORPTANCE_EMITTANCE => "absorptanceEmittance",
            _ => throw new ArgumentOutOfRangeException(nameof(result), $"Unsupported result {result}."),
        };
    }
}