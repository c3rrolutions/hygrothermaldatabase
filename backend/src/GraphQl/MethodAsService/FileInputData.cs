using System.Collections.Generic;

namespace Database.GraphQl.MethodAsService;

public sealed record FileInputData(
    Data Data
);

public sealed record Data(
    ComponentCharacteristics ComponentCharacteristics,
    IReadOnlyList<DataPoint> DataPoints
);

public sealed record ComponentCharacteristics(
    string Surface,
    string Symmetries
);

public sealed record DataPoint(
    Incidence Incidence,
    Emergence Emergence,
    Results Results
);

public sealed record Incidence(
    Wavelengths Wavelengths,
    Direction Direction
);

public sealed record Emergence(
    Direction Direction
);

public sealed record Results(
    double Transmittance,
    double? Reflectance = null,
    ColorTransmittance? ColorTransmittance = null,
    ColorReflectance? ColorReflectance = null
);

public sealed record Direction(
    int Polar
);

public sealed record Wavelengths(
    int Wavelength
);

public record ColorTransmittance(
    IReadOnlyList<CielabColor> CielabColors,
    IReadOnlyList<ColorRenderingIndex> ColorRenderingIndices
);

public record ColorReflectance(
    IReadOnlyList<CielabColor> CielabColors
);

public record CielabColor(
    int LStar,
    double AStar,
    double BStar,
    string Observer,
    string Illuminant
);

public record ColorRenderingIndex(
    int Ra,
    string Observer,
    string Illuminant
);