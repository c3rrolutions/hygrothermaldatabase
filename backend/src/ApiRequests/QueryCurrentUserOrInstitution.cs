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
public sealed class QueryCurrentUserOrInstitution
{
    private const string QueryFileName = "CurrentUserOrInstitution.graphql";

    public static readonly CurrentUserOrInstitution Empty = new(null, null);

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

    public sealed record CurrentInstitution(
        Guid Uuid,
        string Name,
        DatabaseOperatingDatabaseConnection DatabaseOperatingDatabases,
        DatabaseOperatingManagedInstitutionConnection DatabaseOperatingManagedInstitutions
    )
    {
        public bool IsDatabaseOperator()
        {
            return DatabaseOperatingDatabases.TotalCount >= 1
                || DatabaseOperatingManagedInstitutions.TotalCount >= 1;
        }
    };

    public sealed record DatabaseOperatingDatabaseConnection(
        uint TotalCount
    );

    public sealed record DatabaseOperatingManagedInstitutionConnection(
        uint TotalCount
    );

    public sealed record CurrentUserOrInstitution(
        CurrentUser? CurrentUser,
        CurrentInstitution? CurrentInstitution
    );

    /// <summary>
    /// Request current user from Metabase.
    /// </summary>
    public static async Task<CurrentUserOrInstitution> Do(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        CancellationToken cancellationToken
    )
    {
        return (await apiRequestService.QueryGraphQl<CurrentUserOrInstitution>(
            GetGraphQlEndpoint(appSettings),
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(QueryFileName),
                new
                {
                    databaseId = appSettings.DatabaseId
                },
                "CurrentUserOrInstitution"
            ),
            cancellationToken
        ))
        .Data;
    }
}