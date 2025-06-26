static double CalculateWeightedIntegral(List<(double Wavelength, double Intensity)> data, double weight)
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
}