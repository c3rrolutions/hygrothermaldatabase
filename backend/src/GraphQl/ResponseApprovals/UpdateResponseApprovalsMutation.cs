using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut.Data;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using HotChocolate.Data;
using Microsoft.Extensions.Logging;
using Database.Extensions;
using Database.GraphQl.Extensions;
using HotChocolate.Resolvers;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.ResponseApprovals;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateResponseApprovalsErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_DATABASE,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record UpdateResponseApprovalsError(
    UpdateResponseApprovalsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UpdateResponseApprovalsErrorCode>(Code, Message, Path);

public static partial class UpdateResponseApprovalsMutationLogging
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Updating response approval for data of type {Type} with ID {Id} failed."
    )]
    public static partial void FailedUpdatingResponseApproval(this ILogger<UpdateResponseApprovalsMutation> logger, Type type, Guid id, Exception exception);
}

public sealed record UpdateResponseApprovalsPayload(
    IReadOnlyCollection<IData>? Data,
    IReadOnlyCollection<UpdateResponseApprovalsError>? Errors
) : Payload;

public sealed class UpdateResponseApprovalsFilterType
: ResponseApprovalFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<IData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(UpdateResponseApprovalsFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateResponseApprovalsMutation
: DataMutationBase<IReadOnlyCollection<IData>, UpdateResponseApprovalsPayload, UpdateResponseApprovalsError, UpdateResponseApprovalsErrorCode>
{
    protected override UpdateResponseApprovalsPayload NewPayload(
        IReadOnlyCollection<IData>? data,
        IReadOnlyCollection<UpdateResponseApprovalsError>? errors
    ) => new(data, errors);

    protected override UpdateResponseApprovalsError NewError(
        UpdateResponseApprovalsErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [UseFiltering<UpdateResponseApprovalsFilterType>]
    public async Task<UpdateResponseApprovalsPayload> UpdateResponseApprovalsAsync(
        IResolverContext resolverContext,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ILogger<UpdateResponseApprovalsMutation> logger,
        CancellationToken cancellationToken
    )
    {
        var queryContext = resolverContext.GetQueryContext<IData>();
        if ((await AuthorizeAsync(
                UpdateResponseApprovalsErrorCode.UNAUTHENTICATED,
                UpdateResponseApprovalsErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        var dataSets = new ConcurrentBag<IData>();
        var errors = new ConcurrentBag<UpdateResponseApprovalsError>();
        await Parallel.ForEachAsync(
            context.GetAllDataAsync(_ => _.Approval != null, queryContext),
            cancellationToken,
            async (data, cancellationToken) =>
            {
                try
                {
                    data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
                    dataSets.Add(data);
                }
                catch (Exception exception)
                {
                    logger.FailedUpdatingResponseApproval(data.GetType(), data.Id, exception);
                    errors.Add(
                        NewError(
                            UpdateResponseApprovalsErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                            $"Failed creating a response approval for the data set {data.Id} named '{data.Name}' with the error message: {exception.Message}",
                            []
                        )
                    );
                }
            }
        );
        await context.SaveChangesAsync(cancellationToken);
        if (!errors.IsEmpty)
        {
            return NewPayload(dataSets, errors);
        }
        return NewPayload(dataSets, null);
    }
}