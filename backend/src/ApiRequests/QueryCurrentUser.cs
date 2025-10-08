using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

/// <summary>
/// Class to request user info from Metabase API.
/// </summary>
public sealed class QueryCurrentUser
{
    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        ApiRequestService.MetabaseGraphQlEndpoint(appSettings);

    public sealed record CurrentUser(
        Guid Uuid,
        string Name,
        UserRepresentedInstitutionConnection RepresentedInstitutions,
        UserRepresentedInstitutionConnection DatabaseOperatingRepresentedInstitutions
    );
    
    public sealed record UserRepresentedInstitutionConnection(
        IReadOnlyList<UserRepresentedInstitutionEdge> Edges,
        uint TotalCount
    );
    
    public enum InstitutionRepresentativeRole
    {
        OWNER,
        ASSISTANT
    }
    
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

    private sealed record CurrentUserData(CurrentUser CurrentUser);

    private static readonly string[] s_currentUserFileNames =
    [
        "CurrentUser.graphql"
    ];

    /// <summary>
    /// Request current user from Metabase.
    /// </summary>
    public static async Task<CurrentUser?> Do(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        return (await apiRequestService.Metabase().QueryGraphQl<CurrentUserData>(
                   appSettings,
                   new GraphQLRequest(await apiRequestService.ConstructGraphQlQuery(s_currentUserFileNames),
                       new {
                        databaseId = appSettings.DatabaseId
                       },
                       "CurrentUser"
                   ),
                   httpClientFactory,
                   httpContextAccessor,
                   cancellationToken
               ))
               ?.Data
               ?.CurrentUser
               ?? null;
    }
}