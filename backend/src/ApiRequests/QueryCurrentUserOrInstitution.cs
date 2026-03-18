using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Logging;
using Database.Services;
using GraphQL;
using Microsoft.Extensions.Logging;

namespace Database.ApiRequests;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Response contains errors.")
    ]
    internal static partial void ResponseErrors(
        this ILogger<QueryCurrentUserOrInstitution> logger,
        [TagProvider(typeof(GraphQlErrorsTagProvider), nameof(GraphQlErrorsTagProvider.RecordTags))] GraphQLError[] errors
    );
}

/// <summary>
/// Request current user or institution from metabase.
/// </summary>
public sealed class QueryCurrentUserOrInstitution(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    ILogger<QueryCurrentUserOrInstitution> logger
)
{
    private const string QueryFileName = "CurrentUserOrInstitution.graphql";

    public static readonly CurrentUserOrInstitution Empty = new(null, null);

    public Uri GetGraphQlEndpoint =>
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

    public async Task<CurrentUserOrInstitution> Do(
        CancellationToken cancellationToken
    )
    {
        var response = (await apiRequestService.QueryGraphQl<CurrentUserOrInstitution>(
            GetGraphQlEndpoint,
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(QueryFileName),
                new
                {
                    databaseId = appSettings.DatabaseId
                },
                "CurrentUserOrInstitution"
            ),
            cancellationToken
        ));
        if (response.Errors is not null)
        {
            logger.ResponseErrors(response.Errors);
        }
        return response.Data;
    }
}