using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Extensions;
using Database.GraphQl.Extensions;
using Database.Services;
using GreenDonut.Data;
using HotChocolate.Data;
using HotChocolate.Authorization;
using HotChocolate.Data.Filters;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.GraphQl.GetHttpsResources;

[SuppressMessage("Naming", "CA1707")]
public enum RecomputeGetHttpsResourceHashValuesErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    FAILED,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record RecomputeGetHttpsResourceHashValuesError(
    RecomputeGetHttpsResourceHashValuesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<RecomputeGetHttpsResourceHashValuesErrorCode>(Code, Message, Path);

public sealed record RecomputeGetHttpsResourceHashValuesPayload(
   IReadOnlyCollection<GetHttpsResource>? GetHttpsResources,
   IReadOnlyCollection<RecomputeGetHttpsResourceHashValuesError>? Errors
) : Payload;

public sealed class RecomputeGetHttpsResourceHashValuesFilterType
    : GetHttpsResourceFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(RecomputeGetHttpsResourceHashValuesFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Recomputing the hash value of the GET HTTPS resource with ID {Id} failed."
    )]
    public static partial void FailedRecomputingHashValue(this ILogger<RecomputeGetHttpsResourceHashValuesMutation> logger, Guid id, Exception exception);
}

[ExtendObjectType(nameof(Mutation))]
public sealed class RecomputeGetHttpsResourceHashValuesMutation
: DataMutationBase<IReadOnlyCollection<GetHttpsResource>, RecomputeGetHttpsResourceHashValuesPayload, RecomputeGetHttpsResourceHashValuesError, RecomputeGetHttpsResourceHashValuesErrorCode>
{
    protected override RecomputeGetHttpsResourceHashValuesPayload NewPayload(
        IReadOnlyCollection<GetHttpsResource>? data,
        IReadOnlyCollection<RecomputeGetHttpsResourceHashValuesError>? errors
    ) => new(data, errors);

    protected override RecomputeGetHttpsResourceHashValuesError NewError(
        RecomputeGetHttpsResourceHashValuesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [UseFiltering<RecomputeGetHttpsResourceHashValuesFilterType>]
    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<RecomputeGetHttpsResourceHashValuesPayload> RecomputeGetHttpsResourceHashValuesAsync(
        ApplicationDbContext context,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ILogger<RecomputeGetHttpsResourceHashValuesMutation> logger,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                RecomputeGetHttpsResourceHashValuesErrorCode.UNAUTHENTICATED,
                RecomputeGetHttpsResourceHashValuesErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var resources =
            await context.GetHttpsResourcesWithData
            .With(resolverContext.GetQueryContext<GetHttpsResource>(), Sorting.DefaultEntityOrder)
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
                    if ((await CreateResponseApprovalAsync(
                            resource.Data ?? throw new InvalidOperationException("Impossible!"),
                            RecomputeGetHttpsResourceHashValuesErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                            responseApprovalService,
                            context,
                            cancellationToken
                        )
                        ).Failed(out var createResponseApprovalErrorPayload)
                    )
                    {
                        // TODO Undo hash value recomputation?
                        // await context.SaveChangesAsync(cancellationToken);
                        errors.AddRange(createResponseApprovalErrorPayload.Errors);
                    }
                }
                catch (Exception exception)
                {
                    logger.FailedRecomputingHashValue(resource.Id, exception);
                    errors.Add(
                        NewError(
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
            return NewPayload(resources, errors);
        }
        return NewPayload(resources, null);
    }
}
