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

        //// Create example spectralDataPoints
        // // Flat spectrum
        // IReadOnlyList<DataPoint> spectralDataPoints = new List<DataPoint> {
        //     new DataPoint(new Incidence(new Wavelengths(200), new Direction(8)), new Emergence(new Direction(8)), new Results(0.5)),
        //     new DataPoint(new Incidence(new Wavelengths(800), new Direction(8)), new Emergence(new Direction(8)), new Results(0.5)),
        //     new DataPoint(new Incidence(new Wavelengths(810), new Direction(8)), new Emergence(new Direction(8)), new Results(0.5)),
        //     new DataPoint(new Incidence(new Wavelengths(2600), new Direction(8)), new Emergence(new Direction(8)), new Results(0.5)) };
        // // only non-visible values
        // IReadOnlyList<DataPoint> spectralDataPoints = new List<DataPoint> {
        //     new DataPoint(new Incidence(new Wavelengths(200), new Direction(8)), new Emergence(new Direction(8)), new Results(0)),
        //     new DataPoint(new Incidence(new Wavelengths(784), new Direction(8)), new Emergence(new Direction(8)), new Results(0)),
        //     new DataPoint(new Incidence(new Wavelengths(785), new Direction(8)), new Emergence(new Direction(8)), new Results(1)),
        //     new DataPoint(new Incidence(new Wavelengths(2600), new Direction(8)), new Emergence(new Direction(8)), new Results(1)) };
        // only visible values
        IReadOnlyList<DataPoint> spectralDataPoints = new List<DataPoint> {
            new DataPoint(new Incidence(new Wavelengths(200), new Direction(8)), new Emergence(new Direction(8)), new Results(1)),
            new DataPoint(new Incidence(new Wavelengths(784), new Direction(8)), new Emergence(new Direction(8)), new Results(1)),
            new DataPoint(new Incidence(new Wavelengths(785), new Direction(8)), new Emergence(new Direction(8)), new Results(0)),
            new DataPoint(new Incidence(new Wavelengths(2600), new Direction(8)), new Emergence(new Direction(8)), new Results(0)) };

        // Use the SpectralToIntegralMethod
        SpectralToIntegralMethod mySpectralToIntegralMethod = new SpectralToIntegralMethod();
        // foreach (IMethod.StandardType standard in Enum.GetValues<IMethod.StandardType>())
        // {
        List<DataPoint> results = mySpectralToIntegralMethod.Calculate(spectralDataPoints);
        // Console.WriteLine($"Standard: {standard}");
        foreach (DataPoint integralDataPoint in results)
        {
            Console.WriteLine($"integralDataPoint\nIncidence Wavelength: {integralDataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {integralDataPoint.Incidence.Direction.Polar}");
            Console.WriteLine($"Emergence Direction Polar: {integralDataPoint.Emergence.Direction.Polar}");
            Console.WriteLine($"Results Transmittance: {integralDataPoint.Results.Transmittance}\n");
        }
        // }

        await Task.FromResult(0);
    }
}