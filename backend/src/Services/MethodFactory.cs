using System;
using Database.Methods;

namespace Database.Services;

public sealed class MethodFactory
{
    public IMethod? GetMethod(Guid methodId)
    {
        return methodId switch
        {
            var _ when methodId == TransmittanceSumMethod.Id => new TransmittanceSumMethod(),
            var _ when methodId == SpectralToIntegralMethod.Id => new SpectralToIntegralMethod(),
            _ => null,
        }
    }
}