using NUnit.Framework;
using FluentAssertions;
using Database.Methods.SpectralToIntegral;
using System.Threading.Tasks;
using System.Text.Json;
using Database.Methods;
using System.IO;
using System;
using Database.ApiRequests;
using System.Reflection;
using FluentAssertions.Execution;
using System.Diagnostics.CodeAnalysis;

namespace Database.Tests.Methods;

// [Follow test naming standards](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#follow-test-naming-standards)
public sealed class SpectralToIntegralMethodTests
{
    private static string ConstructFilePath(string relativePath)
    {
        var testAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? throw new InvalidOperationException(); ;
        var testProjectRoot = Path.GetFullPath(Path.Combine(testAssemblyPath, @"../../../"));
        return Path.Combine(testProjectRoot, relativePath);
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public void Calculate_None()
    {
        var spectralDataPoints = new SpectralToIntegralInput([
            new SpectralToIntegralData([])
        ]);
        var method = new SpectralToIntegralMethod();
        var results = method.Calculate(spectralDataPoints);
        using (new AssertionScope())
        {
            results.En410Visible.Should().BeApproximately(99.0, 0.000000000000001F);
            results.En410Solar.Should().BeApproximately(98.99999999999999, 0.00000001F);
            results.Iso9050Solar.Should().BeApproximately(98.99999999999999, 0.00000001F);
        }
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public void Calculate_One()
    {
        var spectralDataPoints = new SpectralToIntegralInput([
            new SpectralToIntegralData([
                new DataPoint(new Incidence(new Wavelengths(200)), new Results(1)),
            ])
        ]);
        var method = new SpectralToIntegralMethod();
        var results = method.Calculate(spectralDataPoints);
        using (new AssertionScope())
        {
            results.En410Visible.Should().BeApproximately(50.00000000000002, 0.000000000000001F);
            results.En410Solar.Should().BeApproximately(50.0, 0.00000001F);
            results.Iso9050Solar.Should().BeApproximately(50.0, 0.00000001F);
        }
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public void Calculate_Few()
    {
        var spectralDataPoints = new SpectralToIntegralInput([
            new SpectralToIntegralData([
                new DataPoint(new Incidence(new Wavelengths(200)), new Results(1)),
                new DataPoint(new Incidence(new Wavelengths(784)), new Results(1)),
                new DataPoint(new Incidence(new Wavelengths(785)), new Results(0)),
                new DataPoint(new Incidence(new Wavelengths(2600)), new Results(0))
            ])
        ]);
        var method = new SpectralToIntegralMethod();
        var results = method.Calculate(spectralDataPoints);
        using (new AssertionScope())
        {
            results.En410Visible.Should().BeApproximately(1.0F, 0.000000000000001F);
            results.En410Solar.Should().BeApproximately(0.3733675409177318F, 0.00000001F);
            results.Iso9050Solar.Should().BeApproximately(0.212543149312381F, 0.00000001F);
        }
    }

    [Test]
    [SuppressMessage("Naming", "CA1707")]
    public async Task Calculate_Many()
    {
        using var fileStream = File.OpenRead(
            ConstructFilePath("./Methods/2d40b285-79d4-4ec6-8d46-767bd9b0f249.json")
        );
        using var jsonDocument = await JsonDocument.ParseAsync(
            fileStream,
            JsonDocumentSettings.Lax
        );
        var spectralDataPoints = jsonDocument.RootElement.Deserialize<SpectralToIntegralInput>(
            MethodBase<SpectralToIntegralInput, SpectralToIntegralOutput>.JsonSerializerOptions
        ) ?? throw new InvalidOperationException();
        var method = new SpectralToIntegralMethod();
        var results = method.Calculate(spectralDataPoints);
        using (new AssertionScope())
        {
            results.En410Visible.Should().BeApproximately(0.9023789669000116, 0.000000000000001F);
            results.En410Solar.Should().BeApproximately(0.9080345756086065, 0.00000001F);
            results.Iso9050Solar.Should().BeApproximately(0.8593226930274324, 0.00000001F);
        }
    }
}