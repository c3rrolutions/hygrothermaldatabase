using System.Collections.Generic;

namespace Database.Methods.TransmittanceSum;

public sealed record TransmittanceSumInput(
    Data Data
);

public sealed record Data(
    IReadOnlyList<DataPoint> DataPoints
);

public sealed record DataPoint(
    Results Results
);

public sealed record Results(
    double Transmittance
);