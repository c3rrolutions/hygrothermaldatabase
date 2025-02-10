using System;
using System.Collections.Generic;

namespace Database.Metabase;

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

public record CurrentUser(
    Guid Id,
    Guid Uuid,
    RepresentedInstitutions RepresentedInstitutions
);

public record Edge(
    Node Node,
    InstitutionRepresentativeRole Role,
    DataSigningPermission DataSigningPermission
);

public record Node(
    Guid Id,
    Guid Uuid,
    string Name
);

public record RepresentedInstitutions(
    IReadOnlyList<Edge> Edges
);