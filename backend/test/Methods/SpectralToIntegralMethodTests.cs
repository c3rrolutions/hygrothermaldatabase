using NUnit.Framework;
using FluentAssertions;
using Database.Methods.SpectralToIntegral;

namespace Database.Tests.Methods;

public sealed class SpectralToIntegralMethodTests
{
    [Test]
    public void CalculateTest()
    {
        var spectralDataPoints = new SpectralToIntegralInput(
            new SpectralToIntegralData([
                new DataPoint(new Incidence(new Wavelengths(200)), new Results(1)),
                new DataPoint(new Incidence(new Wavelengths(784)), new Results(1)),
                new DataPoint(new Incidence(new Wavelengths(785)), new Results(0)),
                new DataPoint(new Incidence(new Wavelengths(2600)), new Results(0))
            ])
        );
        var method = new SpectralToIntegralMethod();
        var results = method.Calculate(spectralDataPoints);
        results.En410Visible.Should().BeApproximately(1.0F, 0.000000000000001F);
        results.En410Solar.Should().BeApproximately(0.3733675409177318F, 0.00000001F);
        results.Iso9050Solar.Should().BeApproximately(0.212543149312381F, 0.00000001F);
    }
}