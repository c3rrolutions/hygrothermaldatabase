using System;
using System.Collections.Generic;
using Database.Data;
using Database.Enumerations;

namespace Database.ApiRequests.Dto;

public sealed record GeometricDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethodDto AppliedMethod,
    ResourceTreeDto ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<ResourceDto> Resources,
    IReadOnlyList<string> Warnings,
    IReadOnlyList<double> Thicknesses
);

public sealed record CalorimetricDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethodDto AppliedMethod,
    ResourceTreeDto ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<ResourceDto> Resources,
    IReadOnlyList<string> Warnings,
    IReadOnlyList<double> GValues,
    IReadOnlyList<double> UValues
);

public sealed record HygrothermalDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethodDto AppliedMethod,
    ResourceTreeDto ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<ResourceDto> Resources,
    IReadOnlyList<string> Warnings
);

public sealed record OpticalDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethodDto AppliedMethod,
    ResourceTreeDto ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<ResourceDto> Resources,
    IReadOnlyList<string> Warnings,
    OpticalComponentType? Type,
    OpticalComponentSubtype? Subtype,
    CoatedSide? CoatedSide,
    IReadOnlyList<double> NearnormalHemisphericalVisibleTransmittances,
    IReadOnlyList<double> NearnormalHemisphericalVisibleReflectances,
    IReadOnlyList<double> NearnormalHemisphericalSolarTransmittances,
    IReadOnlyList<double> NearnormalHemisphericalSolarReflectances,
    IReadOnlyList<double> InfraredEmittances,
    IReadOnlyList<double> ColorRenderingIndices,
    ICollection<CielabColor> CielabColors
);

public sealed record PhotovoltaicDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethodDto AppliedMethod,
    ResourceTreeDto ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<ResourceDto> Resources,
    IReadOnlyList<string> Warnings
);

public sealed record AppliedMethodDto(
    string MethodId,
    IReadOnlyList<NamedMethodArgument> Arguments,
    IReadOnlyList<NamedMethodSource> Sources
);

public sealed record ResourceDto(
    string Description,
    string HashValue,
    string Locator,
    Guid DataFormatId
);

public sealed record ResourceTreeDto(
    RootDto Root,
    IReadOnlyList<object> NonRootVertices
);

public sealed record RootDto(
    ResourceDto Value
);