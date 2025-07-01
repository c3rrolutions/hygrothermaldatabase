using System;
using System.Collections.Generic;
using System.Linq;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public sealed class SpectralToIntegralMethod : IMethod
{
    public string Name => "SpectralToIntegral";

    public Guid Id => Guid.Parse("5abb16b2-7161-470c-9744-85dd14b0e637");

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> spectralDataPoints, IReadOnlyList<DataPoint> weightingDataPoints)
    {
        // Filter for spectral data points only, no integral data points.
        List<DataPoint> spectralDataPointsToFilter = new List<DataPoint>(spectralDataPoints);
        // List<DataPoint> weightingDataPointsFiltered = weightingDataPoints.RemoveAll(dataPoint => dataPoint.Incidence.Wavelengths == "integral");

        // Print debug output
        foreach (DataPoint dataPoint in spectralDataPoints)
        {
            Console.WriteLine($"Incidence Wavelength: {dataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {dataPoint.Incidence.Direction.Polar}");
            Console.WriteLine($"Emergence Direction Polar: {dataPoint.Emergence.Direction.Polar}");
            Console.WriteLine($"Results Transmittance: {dataPoint.Results.Transmittance}");
        }

        Console.WriteLine($"\nspectralDataPointsToFilter.TrueForAll(WavelengthOutOfBounds) = {spectralDataPointsToFilter.TrueForAll(WavelengthOutOfBounds)}\n");
        Console.WriteLine($"\nspectralDataPointsToFilter.Find(WavelengthOutOfBounds) = {spectralDataPointsToFilter.Find(WavelengthOutOfBounds)}\n");

        List<DataPoint> spectralDataPointsFiltered = spectralDataPointsToFilter.Where(dataPoint => !WavelengthOutOfBounds(dataPoint)).ToList();

        // List<DataPoint> spectralDataPointsFiltered = spectralDataPointsToFilter.RemoveAll(WavelengthOutOfBounds);

        // Console.WriteLine($"\nspectralDataPointsToFilter.RemoveAll(WavelengthOutOfBounds) = {spectralDataPointsToFilter.RemoveAll(WavelengthOutOfBounds)}\n");
        Console.WriteLine($"\nspectralDataPointsFiltered.Find(WavelengthOutOfBounds) = {spectralDataPointsFiltered.Find(WavelengthOutOfBounds)}\n");

        // Print debug output
        foreach (DataPoint dataPoint in spectralDataPointsFiltered)
        {
            Console.WriteLine($"Incidence Wavelength: {dataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {dataPoint.Incidence.Direction.Polar}");
            Console.WriteLine($"Emergence Direction Polar: {dataPoint.Emergence.Direction.Polar}");
            Console.WriteLine($"Results Transmittance: {dataPoint.Results.Transmittance}");
        }

        // List<DataPoint> spectralDataPointsFiltered = spectralDataPointsToFilter.RemoveAll(WavelengthOutOfBounds);
        // // List<DataPoint> weightingDataPointsFiltered = weightingDataPoints.RemoveAll(dataPoint => dataPoint.Incidence.Wavelengths == "integral");
        // // Sort the dataPoints according to their wavelength
        // List<DataPoint> spectralDataPointsSorted = spectralDataPointsFiltered.OrderByDescending(dataPoint => dataPoint.Incidence.Wavelengths.Wavelength);
        // List<DataPoint> weightingDataPointsSorted = weightingDataPointsFiltered.OrderByDescending(dataPoint => dataPoint.Incidence.Wavelengths.Wavelength);



        double wavelengthWeighting = 0.0D, deltaWavelength = 0.0D, averageValue = 0.0D, numerator = 0.0D, denominator = 0.0D;
        // foreach (DataPoint weightingDataPoint in weightingDataPointsSorted)
        // {
        //     wavelengthWeighting = weightingDataPoint.Incidence.Wavelengths.Wavelength;
        //     DataPoint spectralDataPointWavelengthBelow = new DataPoint(new Incidence(new Wavelengths(0), new Direction(8)), new Emergence(new Direction(8)), new Results(99));
        //     DataPoint spectralDataPointWavelengthAbove = new DataPoint(new Incidence(new Wavelengths(1000000), new Direction(8)), new Emergence(new Direction(8)), new Results(99));
        //     foreach (DataPoint spectralDataPoint in spectralDataPointsSorted)
        //     {
        //         // Find the spectralDataPoints with the wavelengths which are the closest to the wavelength of the weightingDataPoint
        //         if ((spectralDataPoint.Incidence.Wavelengths.Wavelength <= wavelengthWeighting) && (spectralDataPoint.Incidence.Wavelengths.Wavelength > spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength))
        //         { spectralDataPointWavelengthBelow = spectralDataPoint; }
        //         if ((spectralDataPoint.Incidence.Wavelengths.Wavelength > wavelengthWeighting) && (spectralDataPoint.Incidence.Wavelengths.Wavelength < spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength))
        //         { spectralDataPointWavelengthAbove = spectralDataPoint; }
        //     }
        //     // Trapezoidal rule to calculate an integral
        //     deltaWavelength = spectralDataPointWavelengthAbove.Incidence.Wavelengths.Wavelength - spectralDataPointWavelengthBelow.Incidence.Wavelengths.Wavelength;
        //     averageValue = (spectralDataPointWavelengthBelow.Results.Transmittance + spectralDataPointWavelengthAbove.Results.Transmittance) / 2;
        //     numerator += averageValue * deltaWavelength * weightingDataPoint.Results.Transmittance;
        //     denominator += deltaWavelength * weightingDataPoint.Results.Transmittance;
        //     // Print debug output
        //     foreach (DataPoint dataPoint in new DataPoint[] { weightingDataPoint, spectralDataPointWavelengthBelow, spectralDataPointWavelengthAbove })
        //     {
        //         Console.WriteLine($"Incidence Wavelength: {dataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {dataPoint.Incidence.Direction.Polar}");
        //         Console.WriteLine($"Emergence Direction Polar: {dataPoint.Emergence.Direction.Polar}");
        //         Console.WriteLine($"Results Transmittance: {dataPoint.Results.Transmittance}");
        //     }
        //     Console.WriteLine($"deltaWavelength = {deltaWavelength}\naverageValue = {averageValue}\nnumerator = {numerator}\ndenominator = {denominator}\n");
        // }
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