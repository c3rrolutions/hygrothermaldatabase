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
        double integral = 0.0;
        double weight = 1;

        // for (int i = 0; i < spectralDataPoints.Count - 1; i++)
        // {
        //     // Trapezoidal rule to calculate an integral
        //     double deltaX = spectralDataPoints[i + 1].Incidence.Wavelength - spectralDataPoints[i].DataPoint.Incidence.Wavelength;
        //     double averageY = (spectralDataPoints[i].DataPoint.Results.Transmittance + spectralDataPoints[i + 1].DataPoint.Results.Transmittance) / 2;
        //     integral += averageY * deltaX * weight;
        // }

        DataPoint integralDataPoint = new DataPoint(new Incidence(new Wavelengths(0), new Direction(0)), new Emergence(new Direction(0)), new Results(integral));

        return [integralDataPoint];
    }

    /*     static double CalculateWeightedIntegral(List<(double Wavelength, double Intensity)> data, double weight)
        {
            double integral = 0.0;

            for (int i = 0; i < data.Count - 1; i++)
            {
                // Trapezregel zur Integralberechnung
                double deltaX = data[i + 1].Wavelength - data[i].Wavelength;
                double averageY = (data[i].Intensity + data[i + 1].Intensity) / 2;
                integral += averageY * deltaX * weight;
            }

            return integral;
        } */

    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints)
    {
        throw new InvalidOperationException("The SpectralToIntegralMethod needs two lists of dataPoints as input.");
    }

}