using System.Collections.Generic;

namespace Database.Methods.SpectralToIntegral;

public sealed record SpectralToIntegralInput(
    IReadOnlyList<SpectralToIntegralData> Data
);

public sealed record SpectralToIntegralData(
    IReadOnlyList<DataPoint> DataPoints
);

public sealed record DataPoint(
    Incidence Incidence,
    Results Results
);

public sealed record Incidence(
    Wavelengths Wavelengths
);

public sealed record Wavelengths(
    double Wavelength
);

public sealed record Results(
    double Transmittance
);