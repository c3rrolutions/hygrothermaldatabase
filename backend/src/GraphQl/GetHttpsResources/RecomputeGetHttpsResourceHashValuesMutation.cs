using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Authorization;
using Database.Enumerations;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Database.Utilities;
using HotChocolate.Data;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore.Query;
using Database.GraphQl.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace Database.GraphQl.GetHttpsResources;

[SuppressMessage("Naming", "CA1707")]
public enum RecomputeGetHttpsResourceHashValuesErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    FAILED
}

public sealed record RecomputeGetHttpsResourceHashValuesError(
    RecomputeGetHttpsResourceHashValuesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<RecomputeGetHttpsResourceHashValuesErrorCode>(Code, Message, Path);

public sealed class RecomputeGetHttpsResourceHashValuesPayload
    : Payload
{
    public IReadOnlyCollection<GetHttpsResource>? GetHttpsResources { get; }
    public IReadOnlyCollection<RecomputeGetHttpsResourceHashValuesError>? Errors { get; }

    public RecomputeGetHttpsResourceHashValuesPayload(
        IReadOnlyCollection<GetHttpsResource>? getHttpsResources
    )
    {
        GetHttpsResources = getHttpsResources;
    }

    public RecomputeGetHttpsResourceHashValuesPayload(
        RecomputeGetHttpsResourceHashValuesError error
    )
    {
        Errors = [error];
    }

    public RecomputeGetHttpsResourceHashValuesPayload(
        IReadOnlyCollection<GetHttpsResource> getHttpsResources,
        IReadOnlyCollection<RecomputeGetHttpsResourceHashValuesError> errors
    )
    : this(getHttpsResources)
    {
        Errors = errors;
    }
}

public static partial class RecomputeGetHttpsResourceHashValuesMutationLogging
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Recomputing the hash value of the GET HTTPS resource with ID {Id} failed."
    )]
    public static partial void FailedRecomputingHashValue(this ILogger<RecomputeGetHttpsResourceHashValuesMutation> logger, Guid id, Exception exception);
}

[ExtendObjectType(nameof(Mutation))]
public sealed class RecomputeGetHttpsResourceHashValuesMutation
{
    [UseFiltering<RecomputeGetHttpsResourceHashValuesFilterType>]
    public async Task<RecomputeGetHttpsResourceHashValuesPayload> RecomputeGetHttpsResourceHashValuesAsync(
        ApplicationDbContext context,
        QueryContext<GetHttpsResource> queryContext,
        UserService userService,
        CommonAuthorization authorization,
        ILogger<RecomputeGetHttpsResourceHashValuesMutation> logger,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new RecomputeGetHttpsResourceHashValuesPayload(
                new RecomputeGetHttpsResourceHashValuesError(
                    RecomputeGetHttpsResourceHashValuesErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new RecomputeGetHttpsResourceHashValuesPayload(
                new RecomputeGetHttpsResourceHashValuesError(
                    RecomputeGetHttpsResourceHashValuesErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to recompute GET HTTPS resource hash values in this database.",
                    []
                )
            );
        }
        var resources =
            await context.GetHttpsResources
            .With(queryContext, sort => sort.StabilizeOrder())
            .ToListAsync(cancellationToken);
        var errors = new ConcurrentBag<RecomputeGetHttpsResourceHashValuesError>();
        await Parallel.ForEachAsync(
            resources,
            cancellationToken,
            async (resource, cancellationToken) =>
            {
                try
                {
                    await resource.RecomputeHashValue(cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.FailedRecomputingHashValue(resource.Id, exception);
                    errors.Add(
                        new RecomputeGetHttpsResourceHashValuesError(
                            RecomputeGetHttpsResourceHashValuesErrorCode.FAILED,
                            $"Recomputing the hash value for the GET HTTPS resource with ID {resource.Id} failed with message: {exception.Message}",
                            []
                        )
                    );
                }

            }
        );
        await context.SaveChangesAsync(cancellationToken);
        if (!errors.IsEmpty)
        {
            return new RecomputeGetHttpsResourceHashValuesPayload(resources, errors);
        }
        return new RecomputeGetHttpsResourceHashValuesPayload(resources);
    }
}