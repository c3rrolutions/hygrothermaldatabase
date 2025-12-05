using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Database.Methods.SpectralToIntegral;

public sealed class SpectralToIntegralMethod
    : MethodBase<SpectralToIntegralInput, SpectralToIntegralOutput>
{
    public static readonly Guid Id = Guid.Parse("285d172c-9bcf-4c57-9be0-ee95651ba7db");

    public ImmutableArray<(int wavelength, double weight, double deltaWavelength)> En410VisibleWavelengthsWeights => [
            (380, 0, 5),
            (390, 0.0005, 10),
            (400, 0.003, 10),
            (410, 0.0103, 10),
            (420, 0.0352, 10),
            (430, 0.0948, 10),
            (440, 0.2274, 10),
            (450, 0.4192, 10),
            (460, 0.6663, 10),
            (470, 0.985, 10),
            (480, 1.5189, 10),
            (490, 2.1336, 10),
            (500, 3.3491, 10),
            (510, 5.1393, 10),
            (520, 7.0523, 10),
            (530, 8.799, 10),
            (540, 9.4427, 10),
            (550, 9.8077, 10),
            (560, 9.4306, 10),
            (570, 8.6891, 10),
            (580, 7.8994, 10),
            (590, 6.3306, 10),
            (600, 5.3542, 10),
            (610, 4.2491, 10),
            (620, 3.1502, 10),
            (630, 2.0812, 10),
            (640, 1.381, 10),
            (650, 0.807, 10),
            (660, 0.4612, 10),
            (670, 0.2485, 10),
            (680, 0.1255, 10),
            (690, 0.0536, 10),
            (700, 0.0276, 10),
            (710, 0.0146, 10),
            (720, 0.0057, 10),
            (730, 0.0035, 10),
            (740, 0.0021, 10),
            (750, 0.0008, 10),
            (760, 0.0001, 10),
            (770, 0, 10),
            (780, 0, 5)
        ];

    public ImmutableArray<(int wavelength, double weight, double deltaWavelength)> En410SolarWavelengthsWeights => [
            (300, 0.0005, 10),
            (320, 0.0069, 20),
            (340, 0.0122, 20),
            (360, 0.0145, 20),
            (380, 0.0177, 20),
            (400, 0.0235, 20),
            (420, 0.0268, 20),
            (440, 0.0294, 20),
            (460, 0.0343, 20),
            (480, 0.0339, 20),
            (500, 0.0326, 20),
            (520, 0.0318, 20),
            (540, 0.0321, 20),
            (560, 0.0312, 20),
            (580, 0.0294, 20),
            (600, 0.0289, 20),
            (620, 0.0289, 20),
            (640, 0.028, 20),
            (660, 0.0273, 20),
            (680, 0.0246, 20),
            (700, 0.0237, 20),
            (720, 0.022, 20),
            (740, 0.023, 20),
            (760, 0.0199, 20),
            (780, 0.0211, 20),
            (800, 0.033, 35),
            (850, 0.0453, 50),
            (900, 0.0381, 50),
            (950, 0.022, 50),
            (1000, 0.0329, 50),
            (1050, 0.0306, 50),
            (1100, 0.0185, 50),
            (1150, 0.0136, 50),
            (1200, 0.021, 50),
            (1250, 0.0211, 50),
            (1300, 0.0166, 50),
            (1350, 0.0042, 50),
            (1400, 0.001, 50),
            (1450, 0.0044, 50),
            (1500, 0.0095, 50),
            (1550, 0.0123, 50),
            (1600, 0.011, 50),
            (1650, 0.0106, 50),
            (1700, 0.0093, 50),
            (1750, 0.0068, 50),
            (1800, 0.0024, 50),
            (1850, 0.0005, 50),
            (1900, 0.0002, 50),
            (1950, 0.0012, 50),
            (2000, 0.003, 50),
            (2050, 0.0037, 50),
            (2100, 0.0057, 75),
            (2200, 0.0066, 100),
            (2300, 0.006, 100),
            (2400, 0.0041, 100),
            (2500, 0.0006, 50)
        ];

    public ImmutableArray<(int wavelength, double weight, double deltaWavelength)> Iso9050SolarWavelengthsWeights => [
            (300, 0, 5),
            (305, 0.000057, 5),
            (310, 0.000236, 5),
            (315, 0.000554, 5),
            (320, 0.000916, 5),
            (325, 0.001309, 5),
            (330, 0.001914, 5),
            (335, 0.002018, 5),
            (340, 0.002189, 5),
            (345, 0.00226, 5),
            (350, 0.002445, 5),
            (355, 0.002555, 5),
            (360, 0.002683, 5),
            (365, 0.00302, 5),
            (370, 0.003359, 5),
            (375, 0.003509, 5),
            (380, 0.0036, 5),
            (385, 0.003529, 5),
            (390, 0.003551, 5),
            (395, 0.004294, 5),
            (400, 0.007812, 10),
            (410, 0.011638, 10),
            (420, 0.011877, 10),
            (430, 0.011347, 10),
            (440, 0.013246, 10),
            (450, 0.015343, 10),
            (460, 0.016166, 10),
            (470, 0.016178, 10),
            (480, 0.016402, 10),
            (490, 0.015794, 10),
            (500, 0.015801, 10),
            (510, 0.015973, 10),
            (520, 0.015357, 10),
            (530, 0.015867, 10),
            (540, 0.015827, 10),
            (550, 0.015844, 10),
            (560, 0.01559, 10),
            (570, 0.015256, 10),
            (580, 0.014745, 10),
            (590, 0.01433, 10),
            (600, 0.014663, 10),
            (610, 0.01503, 10),
            (620, 0.014859, 10),
            (630, 0.014622, 10),
            (640, 0.014526, 10),
            (650, 0.014445, 10),
            (660, 0.014313, 10),
            (670, 0.014023, 10),
            (680, 0.012838, 10),
            (690, 0.011788, 10),
            (700, 0.012453, 10),
            (710, 0.012798, 10),
            (720, 0.010589, 10),
            (730, 0.011233, 10),
            (740, 0.012175, 10),
            (750, 0.012181, 10),
            (760, 0.009515, 10),
            (770, 0.010479, 10),
            (780, 0.011381, 10),
            (790, 0.011262, 10),
            (800, 0.028718, 50),
            (850, 0.04824, 50),
            (900, 0.040297, 50),
            (950, 0.021384, 50),
            (1000, 0.036097, 50),
            (1050, 0.03411, 50),
            (1100, 0.018861, 50),
            (1150, 0.013228, 50),
            (1200, 0.022551, 50),
            (1250, 0.023376, 50),
            (1300, 0.017756, 50),
            (1350, 0.003743, 50),
            (1400, 0.000741, 50),
            (1450, 0.003792, 50),
            (1500, 0.009693, 50),
            (1550, 0.013693, 50),
            (1600, 0.012203, 50),
            (1650, 0.010615, 50),
            (1700, 0.007256, 50),
            (1750, 0.007183, 50),
            (1800, 0.002157, 50),
            (1850, 0.000398, 50),
            (1900, 0.000082, 50),
            (1950, 0.001087, 50),
            (2000, 0.003024, 50),
            (2050, 0.003988, 50),
            (2100, 0.004229, 50),
            (2150, 0.004142, 50),
            (2200, 0.00369, 50),
            (2250, 0.003592, 50),
            (2300, 0.003436, 50),
            (2350, 0.003163, 50),
            (2400, 0.002233, 50),
            (2450, 0.001202, 50)
        ];

    public override SpectralToIntegralOutput Calculate(SpectralToIntegralInput input)
    {
        // TODO What to do when there is more than 1 item?
        var firstDataItem = input.Data[0];
        return new SpectralToIntegralOutput(
            En410Visible: Calculate(firstDataItem.DataPoints, En410VisibleWavelengthsWeights),
            En410Solar: Calculate(firstDataItem.DataPoints, En410SolarWavelengthsWeights),
            Iso9050Solar: Calculate(firstDataItem.DataPoints, Iso9050SolarWavelengthsWeights)
        );
    }

    public double Calculate(
        IReadOnlyList<DataPoint> spectralDataPoints,
        ImmutableArray<(int wavelength, double weight, double deltaWavelength)> wavelengthsWeights
    )
    {
        if (spectralDataPoints == null || spectralDataPoints.Count == 0)
        {
            throw new ArgumentException("The list `spectralDataPoints` is empty.");
        }
        var sortedSpectralDataPoints =
            spectralDataPoints
            .Where(dataPoint => !WavelengthOutOfBounds(dataPoint))
            .OrderBy(dataPoint => dataPoint.Incidence.Wavelengths.Wavelength)
            .ToList()
            .AsReadOnly();
        var numerator = 0.0D;
        var denominator = 0.0D;
        for (int i = 0; i < (wavelengthsWeights.Length - 1); i++)
        {
            var spectralDataPointWavelengthBelow = new DataPoint(new Incidence(new Wavelengths(0)), new Results(99));
            var spectralDataPointWavelengthAbove = new DataPoint(
                new Incidence(new Wavelengths(1000000)),
                new Results(99)
            );
            foreach (var spectralDataPoint in sortedSpectralDataPoints)
            {
                if (spectralDataPoint.Incidence.Wavelengths.Wavelength <= wavelengthsWeights[i].wavelength
                    && spectralDataPoint.Incidence.Wavelengths.Wavelength > spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength
                )
                {
                    spectralDataPointWavelengthBelow = spectralDataPoint;
                }
                if (spectralDataPoint.Incidence.Wavelengths.Wavelength > wavelengthsWeights[i].wavelength
                    && spectralDataPoint.Incidence.Wavelengths.Wavelength < spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength
                )
                {
                    spectralDataPointWavelengthAbove = spectralDataPoint;
                }
            }
            if (spectralDataPointWavelengthBelow.Results.Transmittance == 99)
            {
                throw new ArgumentException($"`spectralDataPoints` has no data point for the smallest wavelength of `wavelengthsWeights`.\nwavelengthsWeights[i].wavelength {wavelengthsWeights[i].wavelength}\nspectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength {spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength}\nspectralDataPointWavelengthBelow.Results.Transmittance {spectralDataPointWavelengthBelow.Results.Transmittance}\nspectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength {spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength}\nspectralDataPointWavelengthAbove.Results.Transmittance {spectralDataPointWavelengthAbove.Results.Transmittance}\n");
            }
            if (spectralDataPointWavelengthAbove.Results.Transmittance == 99)
            {
                throw new ArgumentException($"`spectralDataPoints` has no data point for the largest wavelength of `wavelengthsWeights`.\nwavelengthsWeights[i].wavelength {wavelengthsWeights[i].wavelength}\nspectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength {spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength}\nspectralDataPointWavelengthBelow.Results.Transmittance {spectralDataPointWavelengthBelow.Results.Transmittance}\nspectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength {spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength}\nspectralDataPointWavelengthAbove.Results.Transmittance {spectralDataPointWavelengthAbove.Results.Transmittance}\n");
            }
            // Trapezoidal rule to calculate an integral
            var averageValue = (
                spectralDataPointWavelengthBelow.Results.Transmittance
                + spectralDataPointWavelengthAbove.Results.Transmittance
            ) / 2;
            numerator += averageValue * wavelengthsWeights[i].deltaWavelength * wavelengthsWeights[i].weight;
            denominator += wavelengthsWeights[i].deltaWavelength * wavelengthsWeights[i].weight;
        }
        // Treat the last wavelengthWeight separately, because it has no spectralDataPointWavelengthAbove
        numerator += spectralDataPoints[spectralDataPoints.Count - 1].Results.Transmittance * wavelengthsWeights[wavelengthsWeights.Length - 1].deltaWavelength * wavelengthsWeights[wavelengthsWeights.Length - 1].weight;
        denominator += wavelengthsWeights[wavelengthsWeights.Length - 1].deltaWavelength * wavelengthsWeights[wavelengthsWeights.Length - 1].weight;
        return numerator / denominator;
    }

    // Search predicate returns true if the wavelength is smaller than 0 nm or larger than 3000 nm.
    private static bool WavelengthOutOfBounds(DataPoint dataPoint)
    {
        return
            dataPoint.Incidence.Wavelengths.Wavelength <= 0
            || dataPoint.Incidence.Wavelengths.Wavelength >= 3000;
    }
}