using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public sealed class TransmittanceSumMethod : IMethod
{
    public string Name => "TransmittanceSum";

    public Guid Id => Guid.Parse("dfa3a7b3-a6da-444c-b43f-5ffd021c4a5c");

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints)
    {
        double transmittanceSum = 0;
        foreach (var dataPoint in dataPoints)
        {
            transmittanceSum += dataPoint.Results.Transmittance;
        }

        return
        [
            new DataPoint(new Incidence(new Wavelengths(0), new Direction(0)), new Emergence(new Direction(0)), new Results(transmittanceSum))
        ];
    }

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> spectralDataPoints, IReadOnlyList<DataPoint> weightingSpectrum)
    {

        throw new InvalidOperationException("The TransmittanceSumMethod cannot be used with two lists of dataPoints as input.");

        return
        [
            new DataPoint(new Incidence(new Wavelengths(0), new Direction(0)), new Emergence(new Direction(0)), new Results(0))
        ];
    }

}