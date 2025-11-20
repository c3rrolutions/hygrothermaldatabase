using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;

namespace Database.ApiRequests;

/// <summary>
/// Class to request user info from Metabase API.
/// </summary>
public sealed class QueryCurrentUserOrApplication
{
    private const string QueryFileName = "CurrentUserOrApplication.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        appSettings.MetabaseGraphQlEndpoint;

    public sealed record CurrentUser(
        Guid Uuid,
        string Name,
        UserRepresentedInstitutionConnection RepresentedInstitutions,
        UserRepresentedInstitutionConnection DatabaseOperatingRepresentedInstitutions
    )
    {
        public bool IsAtLeastAssistantManagerOfDatabaseOperator()
        {
            return DatabaseOperatingRepresentedInstitutions.TotalCount >= 1;
        }
    };

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

    public sealed record CurrentOpenIdConnectApplication(
        Guid Uuid,
        string ClientId,
        OpenIdConnectApplicationOwnerEdge Owner
    )
    {
        public bool IsOwnedByDatabaseOperator()
        {
            return Owner.Node.DatabaseOperatingDatabases.TotalCount >= 1
                || Owner.Node.DatabaseOperatingManagedInstitutions.TotalCount >= 1;
        }
    };

    public sealed record OpenIdConnectApplicationOwnerEdge(
        OpenIdConnectApplicationOwnerNode Node
    );

    public sealed record OpenIdConnectApplicationOwnerNode(
        Guid Uuid,
        string Name,
        DatabaseOperatingDatabaseConnection DatabaseOperatingDatabases,
        DatabaseOperatingManagedInstitutionConnection DatabaseOperatingManagedInstitutions
    );

    public sealed record DatabaseOperatingDatabaseConnection(
        uint TotalCount
    );

    public sealed record DatabaseOperatingManagedInstitutionConnection(
        uint TotalCount
    );

    public sealed record CurrentUserOrApplication(
        CurrentUser? CurrentUser,
        CurrentOpenIdConnectApplication? CurrentApplication
    );

    /// <summary>
    /// Request current user from Metabase.
    /// </summary>
    public static async Task<CurrentUserOrApplication> Do(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        CancellationToken cancellationToken
    )
    {
        return (await apiRequestService.QueryGraphQl<CurrentUserOrApplication>(
            GetGraphQlEndpoint(appSettings),
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(QueryFileName),
                new
                {
                    databaseId = appSettings.DatabaseId
                },
                "CurrentUserOrApplication"
            ),
            cancellationToken
        ))
        .Data;
    }
}