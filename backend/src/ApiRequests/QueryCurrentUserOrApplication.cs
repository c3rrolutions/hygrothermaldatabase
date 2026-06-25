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
        this ILogger<QueryCurrentUserOrApplication> logger,
        [TagProvider(typeof(GraphQlErrorsTagProvider), nameof(GraphQlErrorsTagProvider.RecordTags))] GraphQLError[] errors
    );
}

/// <summary>
/// Request current user or institution from metabase.
/// </summary>
public sealed class QueryCurrentUserOrApplication(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    ILogger<QueryCurrentUserOrApplication> logger
)
{
    private const string QueryFileName = "CurrentUserOrApplication.graphql";

    public static readonly CurrentUserOrApplication Empty = new(null, null);

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
            return DatabaseOperatingRepresentedInstitutions.TotalCount > 0;
        }
    };

    public sealed record UserRepresentedInstitutionConnection(
        IReadOnlyList<UserRepresentedInstitutionEdge> Edges,
        int TotalCount
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
        int TotalCount
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
            return DatabaseOperatingDatabases.TotalCount > 0
                || DatabaseOperatingManagedInstitutions.TotalCount > 0;
        }
    };

    public sealed record DatabaseOperatingDatabaseConnection(
        int TotalCount
    );

    public sealed record DatabaseOperatingManagedInstitutionConnection(
        int TotalCount
    );

    public sealed record CurrentOpenIdConnectApplication(
        string ClientId,
        string? DisplayName,
        CurrentInstitution Owner
    );

    public sealed record CurrentUserOrApplication(
        CurrentUser? CurrentUser,
        CurrentOpenIdConnectApplication? CurrentApplication
    )
    {
        public void Deconstruct(
            out CurrentUser? currentUser,
            out CurrentOpenIdConnectApplication? currentApplication
        )
        {
            currentUser = CurrentUser;
            currentApplication = CurrentApplication;
        }
    };

    public async Task<CurrentUserOrApplication> Do(
        CancellationToken cancellationToken
    )
    {
        var response = (await apiRequestService.QueryGraphQl<CurrentUserOrApplication>(
            GetGraphQlEndpoint,
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(QueryFileName),
                new
                {
                    databaseId = appSettings.DatabaseId
                },
                "CurrentUserOrApplication"
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