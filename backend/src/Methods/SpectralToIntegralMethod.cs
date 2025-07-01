using System;
using System.Collections.Generic;
using System.Linq;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public sealed class SpectralToIntegralMethod : IMethod
{
    public string Name => "SpectralToIntegral";

    public Guid Id => Guid.Parse("5abb16b2-7161-470c-9744-85dd14b0e637");

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> spectralDataPoints, IReadOnlyList<DataPoint> weightingDataPoints, string standard)
    {
        // Filter data points which have a wavelength which is out of bounds.
        List<DataPoint> spectralDataPointsToFilter = new List<DataPoint>(spectralDataPoints);
        List<DataPoint> weightingDataPointsToFilter = new List<DataPoint>(weightingDataPoints);
        List<DataPoint> spectralDataPointsFiltered = spectralDataPointsToFilter.Where(dataPoint => !WavelengthOutOfBounds(dataPoint)).ToList();
        List<DataPoint> weightingDataPointsFiltered = weightingDataPointsToFilter.Where(dataPoint => !WavelengthOutOfBounds(dataPoint)).ToList();
        // Sort the dataPoints
        List<DataPoint> spectralDataPointsSorted = spectralDataPointsFiltered.OrderBy(dataPoint => dataPoint.Incidence.Wavelengths.Wavelength).ToList();
        List<DataPoint> weightingDataPointsSorted = weightingDataPointsFiltered.OrderBy(dataPoint => dataPoint.Incidence.Wavelengths.Wavelength).ToList();
        // // Print filtered and sorted spectralDataPoints
        // foreach (DataPoint dataPoint in spectralDataPointsSorted)
        // {
        //     Console.WriteLine($"Incidence Wavelength: {dataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {dataPoint.Incidence.Direction.Polar}");
        //     Console.WriteLine($"Emergence Direction Polar: {dataPoint.Emergence.Direction.Polar}");
        //     Console.WriteLine($"Results Transmittance: {dataPoint.Results.Transmittance}");
        // }
        double wavelengthWeighting = 0.0D, deltaWavelength = 0.0D, averageValue = 0.0D, numerator = 0.0D, denominator = 0.0D;

        for (int i = 0; i < weightingDataPointsSorted.Count; i++)
        {
            wavelengthWeighting = weightingDataPointsSorted[i].Incidence.Wavelengths.Wavelength;
            DataPoint spectralDataPointWavelengthBelow = new DataPoint(new Incidence(new Wavelengths(0), new Direction(8)), new Emergence(new Direction(8)), new Results(99));
            DataPoint spectralDataPointWavelengthAbove = new DataPoint(new Incidence(new Wavelengths(1000000), new Direction(8)), new Emergence(new Direction(8)), new Results(99));
            for (int j = 0; j < spectralDataPointsSorted.Count; j++)
            {
                // Find the spectralDataPoints with the wavelengths which are the closest to the wavelength of the weightingDataPoint
                if ((spectralDataPointsSorted[j].Incidence.Wavelengths.Wavelength <= wavelengthWeighting) && (spectralDataPointsSorted[j].Incidence.Wavelengths.Wavelength > spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength))
                { spectralDataPointWavelengthBelow = spectralDataPointsSorted[j]; }
                if ((spectralDataPointsSorted[j].Incidence.Wavelengths.Wavelength > wavelengthWeighting) && (spectralDataPointsSorted[j].Incidence.Wavelengths.Wavelength < spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength))
                { spectralDataPointWavelengthAbove = spectralDataPointsSorted[j]; }
            }
            // Trapezoidal rule to calculate an integral
            deltaWavelength = spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength - spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength;
            averageValue = (spectralDataPointWavelengthBelow.Results.Transmittance + spectralDataPointWavelengthAbove.Results.Transmittance) / 2;
            numerator += averageValue * deltaWavelength * weightingDataPointsSorted[i].Results.Transmittance;
            denominator += deltaWavelength * weightingDataPointsSorted[i].Results.Transmittance;
            // Print debug output
            foreach (DataPoint dataPoint in new DataPoint[] { weightingDataPointsSorted[i], spectralDataPointWavelengthBelow, spectralDataPointWavelengthAbove })
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

    // Search predicate returns true if the wavelength is smaller than 0 nm or larger than 2500 nm.
    private static bool WavelengthOutOfBounds(DataPoint dataPoint)
    {
        return (dataPoint.Incidence.Wavelengths.Wavelength <= 0) || (dataPoint.Incidence.Wavelengths.Wavelength >= 2500);
        // return (dataPoint.Incidence.Wavelengths.Wavelength <= 0) || (dataPoint.Incidence.Wavelengths.Wavelength >= 2500) || (dataPoint.Incidence.Wavelengths.Wavelength != null);
    }

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints)
    {
        throw new InvalidOperationException("The SpectralToIntegralMethod needs two lists of dataPoints as input.");
    }

}