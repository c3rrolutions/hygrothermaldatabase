using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.Metabase;

public sealed class QueryingRepresentedInstitutionsByCurrentUser
{
    private static readonly string[] _representedInstitutionsByCurrentUserFileNames =
    {
        "RepresentedInstitutionsByCurrentUser.graphql"
    };

    private sealed record Institution(Guid Uuid);
    private sealed record UserRepresentedInstitutionEdge(Institution Node, InstitutionRepresentativeRole Role, DataSigningPermission DataSigningPermission);
    private sealed record UserRepresentedInstitutionConnection(IReadOnlyList<UserRepresentedInstitutionEdge> Edges);
    private sealed record User(UserRepresentedInstitutionConnection RepresentedInstitutions);
    private sealed record RepresentedInstitutionsByCurrentUser(User CurrentUser);

    internal static async Task<IReadOnlyCollection<(Guid Id, InstitutionRepresentativeRole Role, DataSigningPermission Permission)>> Query(
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        return (await QueryingMetabase.QueryGraphQl<RepresentedInstitutionsByCurrentUser>(
                   appSettings,
                   new GraphQLRequest(
                       await QueryingMetabase.ConstructGraphQlQuery(
                           _representedInstitutionsByCurrentUserFileNames
                       ).ConfigureAwait(false),
                       new {},
                       "RepresentedInstitutionsByCurrentUser"
                   ),
                   httpClientFactory,
                   httpContextAccessor,
                   cancellationToken
               ).ConfigureAwait(false))
               ?.Data
               ?.CurrentUser
               ?.RepresentedInstitutions
               ?.Edges
               ?.Select(edge => (edge.Node.Uuid, edge.Role, edge.DataSigningPermission))
               .ToList().AsReadOnly()
               ?? Array.Empty<(Guid Id, InstitutionRepresentativeRole Role, DataSigningPermission Permission)>().AsReadOnly();
    }
}