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

public sealed record ColorTransmittance(
    IReadOnlyList<MethodAsServiceCielabColor> CielabColors,
    IReadOnlyList<ColorRenderingIndex> ColorRenderingIndices
);

public sealed record ColorReflectance(
    IReadOnlyList<MethodAsServiceCielabColor> CielabColors
);

public sealed record MethodAsServiceCielabColor(
    int LStar,
    double AStar,
    double BStar,
    string Observer,
    string Illuminant
);

public sealed record ColorRenderingIndex(
    int Ra,
    string Observer,
    string Illuminant
);