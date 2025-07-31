using System;
using System.Collections.Generic;

namespace Database.ApiRequests.Dto;

public enum InstitutionRepresentativeRole
{
    OWNER,
    ASSISTANT
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711")]
public enum DataSigningPermission
{
    NEVER,
    GRANTED,
    REMOVED
}

public sealed record CurrentUserDto(
    Guid Uuid,
    UserRepresentedInstitutionConnection RepresentedInstitutions
);

public sealed record UserRepresentedInstitutionConnection(
    IReadOnlyList<UserRepresentedInstitutionEdge> Edges
);

public sealed record UserRepresentedInstitutionEdge(
    UserRepresentedInstitutionNode Node,
    InstitutionRepresentativeRole Role,
    DataSigningPermission DataSigningPermission
);

public sealed record UserRepresentedInstitutionNode(
    Guid Uuid,
    string Name,
    InstitutionManagedInstitutionConnection ManagedInstitutions
);

public sealed record InstitutionManagedInstitutionConnection(
    IReadOnlyList<InstitutionManagedInstitutionEdge> Edges
);

public sealed record InstitutionManagedInstitutionEdge(
    InstitutionManagedInstitutionNode Node
);

public sealed record InstitutionManagedInstitutionNode(
    Guid Uuid,
    string Name
);