using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut.Data;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using Microsoft.Extensions.Logging;
using Database.GraphQl.Extensions;
using Database.Extensions;

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
        Message = "Updating response approval for data of type {Type} with ID {Id} failed with the errors {Errors}."
    )]
    public static partial void FailedUpdatingResponseApproval(this ILogger<UpdateResponseApprovalsMutation> logger, Type type, Guid id, IEnumerable<string>? errors);
}

public sealed record UpdateResponseApprovalsPayload(
    IReadOnlyCollection<IData>? Data,
    IReadOnlyCollection<UpdateResponseApprovalsError>? Errors
) : Payload;

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
        QueryContext<IData> queryContext,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ILogger<UpdateResponseApprovalsMutation> logger,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UpdateResponseApprovalsErrorCode.UNAUTHENTICATED,
                UpdateResponseApprovalsErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUser, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        var dataSets = new ConcurrentBag<IData>();
        var errors = new ConcurrentBag<UpdateResponseApprovalsError>();
        await Parallel.ForEachAsync(
            (
                context.CalorimetricData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval != null)
                .ToAsyncEnumerable()
            )
            .Union(
                context.GeometricData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval != null)
                .ToAsyncEnumerable()
            )
            .Union(
                context.HygrothermalData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval != null)
                .ToAsyncEnumerable()
            )
            .Union(
                context.OpticalData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval != null)
                .ToAsyncEnumerable()
            )
            .Union(
                context.PhotovoltaicData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval != null)
                .ToAsyncEnumerable()
            ),
            cancellationToken,
            async (data, cancellationToken) =>
            {
                if ((await CreateResponseApprovalAsync(
                        data,
                        UpdateResponseApprovalsErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                        responseApprovalService,
                        context,
                        cancellationToken
                    )
                    ).Failed(out var createResponseApprovalErrorPayload)
                )
                {
                    logger.FailedUpdatingResponseApproval(data.GetType(), data.Id, createResponseApprovalErrorPayload.Errors?.Select(_ => _.Message));
                    errors.AddRange(createResponseApprovalErrorPayload.Errors);
                }
                else
                {
                    dataSets.Add(data);
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