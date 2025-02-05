using System;
using System.Collections.Generic;
using Database.GraphQl.Approvals;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.OpticalDataX;

public sealed record CreateOpticalDataInput(
    // TODO Why does specifying the type with an attribute not work here?
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    IReadOnlyList<DataApprovalInput> Approvals,
    // ResponseApproval Approval
    RootGetHttpsResourceInput RootResource,
    double[] NearnormalHemisphericalVisibleTransmittances,
    double[] NearnormalHemisphericalVisibleReflectances,
    double[] NearnormalHemisphericalSolarTransmittances,
    double[] NearnormalHemisphericalSolarReflectances,
    double[] InfraredEmittances,
    double[] ColorRenderingIndices,
    IReadOnlyList<CielabColorInput> CielabColors
);