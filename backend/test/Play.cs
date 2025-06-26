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

        // Create example myDataPoints
        var direction = new Direction(8);
        var wavelengths0 = new Wavelengths(450);
        var incidence0 = new Incidence(wavelengths0, direction);
        var emergence0 = new Emergence(direction);
        var results0 = new Results(0.5);
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
        IReadOnlyList<DataPoint> myDataPoints = new List<DataPoint> { dataPoint0, dataPoint1, dataPoint2 };
        Console.WriteLine(myDataPoints);

        // Use the TransmittanceSumMethod
        TransmittanceSumMethod myTransmittanceSumMethod = new TransmittanceSumMethod();
        List<DataPoint> transmittanceSumOutput = myTransmittanceSumMethod.Calculate(myDataPoints);
        Console.WriteLine(transmittanceSumOutput);
        foreach (var dataPoint in transmittanceSumOutput)
        {
            Console.WriteLine($"Incidence Wavelength: {dataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {dataPoint.Incidence.Direction.Polar}");
            Console.WriteLine($"Emergence Direction Polar: {dataPoint.Emergence.Direction.Polar}");
            Console.WriteLine($"Results Transmittance: {dataPoint.Results.Transmittance}");
        }

        await Task.FromResult(0);
    }
}