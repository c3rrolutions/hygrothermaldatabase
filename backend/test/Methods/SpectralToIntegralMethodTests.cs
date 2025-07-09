using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Database.Methods;
using Database.GraphQl.MethodAsService;
using System.Collections.Generic;
using FluentAssertions;

namespace Database.Tests.Methods;

public sealed class SpectralToIntegralMethodTests
{
    [Test]
    public void CalculateTest()
    {
        //// Create example spectralDataPoints
        // only visible values
        IReadOnlyList<DataPoint> spectralDataPoints = new List<DataPoint> {
            new DataPoint(new Incidence(new Wavelengths(200), new Direction(8)), new Emergence(new Direction(8)), new Results(1)),
            new DataPoint(new Incidence(new Wavelengths(784), new Direction(8)), new Emergence(new Direction(8)), new Results(1)),
            new DataPoint(new Incidence(new Wavelengths(785), new Direction(8)), new Emergence(new Direction(8)), new Results(0)),
            new DataPoint(new Incidence(new Wavelengths(2600), new Direction(8)), new Emergence(new Direction(8)), new Results(0)) };

        // Use the SpectralToIntegralMethod
        SpectralToIntegralMethod mySpectralToIntegralMethod = new SpectralToIntegralMethod();
        List<DataPoint> results = mySpectralToIntegralMethod.Calculate(spectralDataPoints);
        foreach (DataPoint integralDataPoint in results)
        {
            if (integralDataPoint.Incidence.Wavelengths.Wavelength==0)
            {integralDataPoint.Results.Transmittance.Should().BeApproximately(1.0F, 0.000000000000001F);}
            if (integralDataPoint.Incidence.Wavelengths.Wavelength==1)
            {integralDataPoint.Results.Transmittance.Should().BeApproximately(0.3733675409177318F, 0.00000001F);}
            if (integralDataPoint.Incidence.Wavelengths.Wavelength==2)
            {integralDataPoint.Results.Transmittance.Should().BeApproximately(0.212543149312381F, 0.00000001F);}
            // Console.WriteLine($"integralDataPoint\nIncidence Wavelength: {integralDataPoint.Incidence.Wavelengths.Wavelength}, Direction Polar: {integralDataPoint.Incidence.Direction.Polar}");
            // Console.WriteLine($"Emergence Direction Polar: {integralDataPoint.Emergence.Direction.Polar}");
            // Console.WriteLine($"Results Transmittance: {integralDataPoint.Results.Transmittance}\n");
        }
    }
}