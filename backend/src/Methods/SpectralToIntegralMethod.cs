using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public sealed class SpectralToIntegralMethod : IMethod
{
    public string Name => "SpectralToIntegral";

    public Guid Id => Guid.Parse("5abb16b2-7161-470c-9744-85dd14b0e637");

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> spectralDataPoints, IReadOnlyList<DataPoint> weightingDataPoints)
    {
        // TODO filter for spectral data points only, no integral data points.
        double wavelengthWeighting = 0.0D, deltaWavelength = 0.0D, averageValue = 0.0D, numerator = 0.0D, denominator = 0.0D;
        foreach (DataPoint weightingDataPoint in weightingDataPoints)
        {
            wavelengthWeighting = weightingDataPoint.Incidence.Wavelengths.Wavelength;
            DataPoint spectralDataPointWavelengthBelow = new DataPoint(new Incidence(new Wavelengths(0), new Direction(8)), new Emergence(new Direction(8)), new Results(99));
            DataPoint spectralDataPointWavelengthAbove = new DataPoint(new Incidence(new Wavelengths(1000000), new Direction(8)), new Emergence(new Direction(8)), new Results(99));
            foreach (DataPoint spectralDataPoint in spectralDataPoints)
            {
                // Find the spectralDataPoints with the wavelengths which are the closest to the wavelength of the weightingDataPoint
                if ((spectralDataPoint.Incidence.Wavelengths.Wavelength <= wavelengthWeighting) && (spectralDataPoint.Incidence.Wavelengths.Wavelength > spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength))
                { spectralDataPointWavelengthBelow = spectralDataPoint; }
                if ((spectralDataPoint.Incidence.Wavelengths.Wavelength > wavelengthWeighting) && (spectralDataPoint.Incidence.Wavelengths.Wavelength < spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength))
                { spectralDataPointWavelengthAbove = spectralDataPoint; }
            }
            // Trapezoidal rule to calculate an integral
            deltaWavelength = spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength - spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength;
            averageValue = (spectralDataPointWavelengthBelow.Results.Transmittance + spectralDataPointWavelengthAbove.Results.Transmittance) / 2;
            numerator += averageValue * deltaWavelength * weightingDataPoint.Results.Transmittance;
            denominator += deltaWavelength * weightingDataPoint.Results.Transmittance;
            // Print debug output
            foreach (DataPoint dataPoint in new DataPoint[] { weightingDataPoint, spectralDataPointWavelengthBelow, spectralDataPointWavelengthAbove })
            {
                Console.WriteLine($"Incidence Wavelength: {dataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {dataPoint.Incidence.Direction.Polar}");
                Console.WriteLine($"Emergence Direction Polar: {dataPoint.Emergence.Direction.Polar}");
                Console.WriteLine($"Results Transmittance: {dataPoint.Results.Transmittance}");
            }
            Console.WriteLine($"deltaWavelength = {deltaWavelength}\naverageValue = {averageValue}\nnumerator = {numerator}\ndenominator = {denominator}\n");
        }
        DataPoint integralDataPoint = new DataPoint(new Incidence(new Wavelengths(0), new Direction(0)), new Emergence(new Direction(0)), new Results(numerator / denominator));

        return [integralDataPoint];
    }

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints)
    {
        throw new InvalidOperationException("The SpectralToIntegralMethod needs two lists of dataPoints as input.");
    }

}