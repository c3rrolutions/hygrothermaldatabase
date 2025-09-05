using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public sealed class TransmittanceSumMethod : IMethod
{
    public string Name => "TransmittanceSum";

    public Guid Id => Guid.Parse("8a7684c7-aaca-4057-8a30-1cb951c2c6a0");

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
}