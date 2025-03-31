using System;
using System.Collections.Generic;

namespace Database.ApiRequests.Dto;

public enum InstitutionRepresentativeRole
{
    OWNER,
    ASSISTANT
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Bezeichner dürfen kein falsches Suffix aufweisen", Justification = "<Ausstehend>")]
public enum DataSigningPermission
{
    NEVER,
    GRANTED,
    REMOVED
}

public sealed record CurrentUserDto(
    Guid Id,
    Guid Uuid,
    RepresentedInstitutionsDto RepresentedInstitutions
);

public sealed record RepresentedInstitutionsEdge(
    NodeDto Node,
    InstitutionRepresentativeRole Role,
    DataSigningPermission DataSigningPermission
);

public sealed record NodeDto(
    Guid Id,
    Guid Uuid,
    string Name
);

public sealed record RepresentedInstitutionsDto(
    IReadOnlyList<RepresentedInstitutionsEdge> Edges
);