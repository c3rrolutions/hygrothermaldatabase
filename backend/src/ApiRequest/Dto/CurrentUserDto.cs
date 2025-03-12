using System;
using System.Collections.Generic;

namespace Database.ApiRequest.Dto;

public enum InstitutionRepresentativeRole
{
    OWNER,
    ASSISTANT
}

public enum DataSigningPermission
{
    NEVER,
    GRANTED,
    REMOVED
}

public sealed record CurrentUserDto(
    Guid Id,
    Guid Uuid,
    RepresentedInstitutions RepresentedInstitutions
);

public sealed record Edge(
    Node Node,
    InstitutionRepresentativeRole Role,
    DataSigningPermission DataSigningPermission
);

public sealed record Node(
    Guid Id,
    Guid Uuid,
    string Name
);

public sealed record RepresentedInstitutions(
    IReadOnlyList<Edge> Edges
);