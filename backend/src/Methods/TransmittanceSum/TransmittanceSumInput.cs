using System.Collections.Generic;

namespace Database.Methods.TransmittanceSum;

public sealed record TransmittanceSumInput(
    IReadOnlyList<TransmittanceSumData> Data
);

public sealed record TransmittanceSumData(
    IReadOnlyList<DataPoint> DataPoints
);

public sealed record DataPoint(
    Results Results
);

public sealed record Results(
    double Transmittance
);