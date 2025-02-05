using System;
using System.Collections.Generic;
using Database.GraphQl.Approvals;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.GeometricDataX;

public sealed record CreateGeometricDataInput(
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    IReadOnlyList<DataApprovalInput> Approvals,
    // ResponseApproval Approval,
    RootGetHttpsResourceInput RootResource,
    double[] Thicknesses
);