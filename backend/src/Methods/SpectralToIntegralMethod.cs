using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public sealed class SpectralToIntegralMethod : IMethod
{
    public string Name => "SpectralToIntegral";

    public Guid Id => Guid.Parse("5abb16b2-7161-470c-9744-85dd14b0e637");

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> spectralDataPoints, IReadOnlyList<DataPoint> weightingSpectrum)
    {
        double transmittanceSum = 0;
        foreach (var dataPoint in spectralDataPoints)
        {
            transmittanceSum += dataPoint.Results.Transmittance;
        }

        DataPoint integralDataPoint = new DataPoint(new Incidence(new Wavelengths(0), new Direction(0)), new Emergence(new Direction(0)), new Results(transmittanceSum));

        return [integralDataPoint];
    }

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints)
    {
        Console.WriteLine("The SpectralToIntegralMethod needs two list of dataPoints as input.");

        return
        [
            new DataPoint(new Incidence(new Wavelengths(0), new Direction(0)), new Emergence(new Direction(0)), new Results(0))
        ];
    }

}