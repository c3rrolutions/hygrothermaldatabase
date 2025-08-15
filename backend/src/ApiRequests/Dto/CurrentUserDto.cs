using System;
using System.Collections.Generic;

namespace Database.ApiRequests.Dto;

public enum InstitutionRepresentativeRole
{
    OWNER,
    ASSISTANT
}

public sealed record CurrentUserDto(
    Guid Uuid,
    string Name,
    UserRepresentedInstitutionConnection RepresentedInstitutions,
    UserRepresentedInstitutionConnection DatabaseOperatingRepresentedInstitutions
);

public sealed record UserRepresentedInstitutionConnection(
    IReadOnlyList<UserRepresentedInstitutionEdge> Edges,
    uint TotalCount
);

public sealed record UserRepresentedInstitutionEdge(
    UserRepresentedInstitutionNode Node,
    InstitutionRepresentativeRole Role
);

public sealed record UserRepresentedInstitutionNode(
    Guid Uuid,
    string Name,
    InstitutionManagedInstitutionConnection ManagedInstitutions
);

public sealed record InstitutionManagedInstitutionConnection(
    IReadOnlyList<InstitutionManagedInstitutionEdge> Edges,
    uint TotalCount
);

public sealed record InstitutionManagedInstitutionEdge(
    InstitutionManagedInstitutionNode Node
);

public sealed record InstitutionManagedInstitutionNode(
    Guid Uuid,
    string Name
);