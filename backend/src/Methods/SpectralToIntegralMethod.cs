using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public sealed class SpectralToIntegralMethod : IMethod
{
    public string Name => "SpectralToIntegral";
    public Guid Id => Guid.Parse("5abb16b2-7161-470c-9744-85dd14b0e637");
    private List<(int wavelength, double weight, double deltaWavelength)> en410WavelengthsWeightsList = new List<(int, double, double)>
        {
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
        };
    public IReadOnlyList<(int wavelength, double weight, double deltaWavelength)> en410WavelengthsWeights { get; }

    public SpectralToIntegralMethod()
    {
        en410WavelengthsWeights = new ReadOnlyCollection<(int, double, double)>(en410WavelengthsWeightsList);
    }

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> spectralDataPoints, string standard)
    {
        // Turn IReadOnlyLists into more flexible Lists
        List<DataPoint> spectralDataPointsToFilter = new List<DataPoint>(spectralDataPoints);
        List<(int wavelength, double weight, double deltaWavelength)> wavelengthsWeights = new List<(int, double, double)> { (0, 0, 0) };
        switch (standard)
        {
            case "EN410":
                wavelengthsWeights = en410WavelengthsWeights.ToList();
                break;
        }
        // Filter data points which have a wavelength which is out of bounds.
        List<DataPoint> spectralDataPointsFiltered = spectralDataPointsToFilter.Where(dataPoint => !WavelengthOutOfBounds(dataPoint)).ToList();
        // Sort the dataPoints
        List<DataPoint> spectralDataPointsSorted = spectralDataPointsFiltered.OrderBy(dataPoint => dataPoint.Incidence.Wavelengths.Wavelength).ToList();
        // // Print filtered and sorted spectralDataPoints
        // foreach (DataPoint dataPoint in spectralDataPointsSorted)
        // {
        //     Console.WriteLine($"Incidence Wavelength: {dataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {dataPoint.Incidence.Direction.Polar}");
        //     Console.WriteLine($"Emergence Direction Polar: {dataPoint.Emergence.Direction.Polar}");
        //     Console.WriteLine($"Results Transmittance: {dataPoint.Results.Transmittance}");
        // }
        double wavelengthWeighting = 0.0D, deltaWavelength = 0.0D, averageValue = 0.0D, numerator = 0.0D, denominator = 0.0D;

        for (int i = 0; i < 2/*wavelengthsWeights.Count*/; i++)
        {
            wavelengthWeighting = wavelengthsWeights[i].wavelength;
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
            // Calculate width of the wavelength interval at weightingDataPointsSorted[i]
            switch (standard)
            {
                case "EN410":
                    deltaWavelength = 10;
                    break;
            }
            // Trapezoidal rule to calculate an integral
            averageValue = (spectralDataPointWavelengthBelow.Results.Transmittance + spectralDataPointWavelengthAbove.Results.Transmittance) / 2;
            numerator += averageValue * deltaWavelength * wavelengthsWeights[i].weight;
            denominator += deltaWavelength * wavelengthsWeights[i].weight;
            // Print debug output
            foreach (DataPoint dataPoint in new DataPoint[] { spectralDataPointWavelengthBelow, spectralDataPointWavelengthAbove })
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

    // Search predicate returns true if the wavelength is smaller than 0 nm or larger than 3000 nm.
    private static bool WavelengthOutOfBounds(DataPoint dataPoint)
    {
        return (dataPoint.Incidence.Wavelengths.Wavelength <= 0) || (dataPoint.Incidence.Wavelengths.Wavelength >= 3000);
    }

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints)
    {
        throw new InvalidOperationException("The SpectralToIntegralMethod needs two lists of dataPoints as input.");
    }

}