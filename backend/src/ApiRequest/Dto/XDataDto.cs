using System;
using System.Collections.Generic;
using Database.Data;
using Database.Enumerations;

namespace Database.ApiRequest.Dto;

public record GeometricDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethod AppliedMethod,
    ResourceTree ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<Resource> Resources,
    IReadOnlyList<string> Warnings,
    IReadOnlyList<double> Thicknesses
);

public record CalorimetricDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethod AppliedMethod,
    ResourceTree ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<Resource> Resources,
    IReadOnlyList<string> Warnings,
    IReadOnlyList<double> GValues,
    IReadOnlyList<double> UValues
);

public record HygrothermalDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethod AppliedMethod,
    ResourceTree ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<Resource> Resources,
    IReadOnlyList<string> Warnings
);

public record OpticalDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethod AppliedMethod,
    ResourceTree ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<Resource> Resources,
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

public record PhotovoltaicDataDto(
    string Uuid,
    DateTime Timestamp,
    string ComponentId,
    string Name,
    string Description,
    AppliedMethod AppliedMethod,
    ResourceTree ResourceTree,
    IReadOnlyList<DataApproval> Approvals,
    DateTime CreatedAt,
    string CreatorId,
    string Locale,
    IReadOnlyList<Resource> Resources,
    IReadOnlyList<string> Warnings
);

public record AppliedMethod(
    string MethodId,
    IReadOnlyList<NamedMethodArgument> Arguments,
    IReadOnlyList<NamedMethodSource> Sources
);

public record Resource(
    string Description,
    string HashValue,
    string Locator,
    Guid DataFormatId
);

public record ResourceTree(
    Root Root,
    IReadOnlyList<object> NonRootVertices
);

public record Root(
    object Value
);