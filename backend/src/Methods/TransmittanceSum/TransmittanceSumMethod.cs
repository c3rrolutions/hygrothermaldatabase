using System;

namespace Database.Methods.TransmittanceSum;

public sealed class TransmittanceSumMethod
    : MethodBase<TransmittanceSumInput, double>
{
    public static readonly Guid Id = Guid.Parse("8a7684c7-aaca-4057-8a30-1cb951c2c6a0");

    public override double Calculate(TransmittanceSumInput input)
    {
        double transmittanceSum = 0;
        foreach (var dataPoint in input.Data.DataPoints)
        {
            transmittanceSum += dataPoint.Results.Transmittance;
        }
        return transmittanceSum;
    }
}