using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Database.Methods;
using Database.GraphQl.MethodAsService;
using System.Collections.Generic;

namespace Database.Tests;

public sealed class Play
{
    [Test]
    public async Task Do()
    {
        Console.WriteLine("Do play!");

        // Create example spectralDataPoints
        var direction = new Direction(8);
        var wavelengths0 = new Wavelengths(450);
        var incidence0 = new Incidence(wavelengths0, direction);
        var emergence0 = new Emergence(direction);
        var results0 = new Results(0.1);
        var wavelengths1 = new Wavelengths(550);
        var incidence1 = new Incidence(wavelengths1, direction);
        var emergence1 = new Emergence(direction);
        var results1 = new Results(0.7);
        var wavelengths2 = new Wavelengths(650);
        var incidence2 = new Incidence(wavelengths2, direction);
        var emergence2 = new Emergence(direction);
        var results2 = new Results(0.8);
        var dataPoint0 = new DataPoint(incidence0, emergence0, results0);
        var dataPoint1 = new DataPoint(incidence1, emergence1, results1);
        var dataPoint2 = new DataPoint(incidence2, emergence2, results2);
        IReadOnlyList<DataPoint> spectralDataPoints = new List<DataPoint> { dataPoint0, dataPoint1, dataPoint2 };

        // Create example weightingSpectrum
        DataPoint dataPoint10 = new DataPoint(new Incidence(new Wavelengths(500), direction), new Emergence(direction), new Results(2));
        DataPoint dataPoint11 = new DataPoint(new Incidence(new Wavelengths(600), direction), new Emergence(direction), new Results(0.5));
        IReadOnlyList<DataPoint> weightingSpectrum = new List<DataPoint> { dataPoint10, dataPoint11 };

        // Use the SpectralToIntegralMethod
        SpectralToIntegralMethod mySpectralToIntegralMethod = new SpectralToIntegralMethod();
        List<DataPoint> integralDataPoints = mySpectralToIntegralMethod.Calculate(spectralDataPoints, weightingSpectrum);
        Console.WriteLine(integralDataPoints);
        foreach (DataPoint integralDataPoint in integralDataPoints)
        {
            Console.WriteLine($"Incidence Wavelength: {integralDataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {integralDataPoint.Incidence.Direction.Polar}");
            Console.WriteLine($"Emergence Direction Polar: {integralDataPoint.Emergence.Direction.Polar}");
            Console.WriteLine($"Results Transmittance: {integralDataPoint.Results.Transmittance}");
        }

        await Task.FromResult(0);
    }
}