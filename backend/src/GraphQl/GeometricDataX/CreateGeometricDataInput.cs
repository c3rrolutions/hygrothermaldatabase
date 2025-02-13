using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Types;
using Database.Enumerations;

namespace Database.GraphQl.GeometricDataX;

public sealed record CreateGeometricDataInput(
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    DataType? Type,
    DataSubtype? Subtype,
    CoatedSide? CoatedSide,
    AppliedMethodInput AppliedMethod,
    IReadOnlyList<DataApprovalInput> Approvals,
    // ResponseApproval Approval,
    RootGetHttpsResourceInput RootResource,
    double[] Thicknesses
);