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
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Data.Filters;
using Microsoft.Extensions.Logging;
using Database.GraphQl.Extensions;
using Database.Extensions;

namespace Database.GraphQl.ResponseApprovals;

[SuppressMessage("Naming", "CA1707")]
public enum CreateResponseApprovalsErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_DATABASE,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreateResponseApprovalsError(
    CreateResponseApprovalsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateResponseApprovalsErrorCode>(Code, Message, Path);

public static partial class CreateResponseApprovalsMutationLogging
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Creating response approval for data of type {Type} with ID {Id} failed with the errors {Errors}."
    )]
    public static partial void FailedCreatingResponseApproval(this ILogger<CreateResponseApprovalsMutation> logger, Type type, Guid id, IEnumerable<string>? errors);
}

public sealed record CreateResponseApprovalsPayload(
    IReadOnlyCollection<IData>? Data,
    IReadOnlyCollection<CreateResponseApprovalsError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateResponseApprovalsMutation
: DataMutationBase<IReadOnlyCollection<IData>, CreateResponseApprovalsPayload, CreateResponseApprovalsError, CreateResponseApprovalsErrorCode>
{
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    protected override CreateResponseApprovalsPayload NewPayload(
        IReadOnlyCollection<IData>? data,
        IReadOnlyCollection<CreateResponseApprovalsError>? errors
    ) => new(data, errors);

    protected override CreateResponseApprovalsError NewError(
        CreateResponseApprovalsErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [UseFiltering<CreateResponseApprovalsFilterType>]
    public async Task<CreateResponseApprovalsPayload> CreateResponseApprovalsAsync(
        QueryContext<IData> queryContext,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ILogger<CreateResponseApprovalsMutation> logger,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                CreateResponseApprovalsErrorCode.UNAUTHENTICATED,
                CreateResponseApprovalsErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUser, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var dataSets = new ConcurrentBag<IData>();
        var errors = new ConcurrentBag<CreateResponseApprovalsError>();
        await Parallel.ForEachAsync(
            (
                context.CalorimetricData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval == null)
                .ToAsyncEnumerable()
            )
            .Union(
                context.GeometricData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval == null)
                .ToAsyncEnumerable()
            )
            .Union(
                context.HygrothermalData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval == null)
                .ToAsyncEnumerable()
            )
            .Union(
                context.OpticalData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval == null)
                .ToAsyncEnumerable()
            )
            .Union(
                context.PhotovoltaicData.AsQueryable<IData>()
                .With(queryContext, sort => sort.StabilizeOrder())
                .Where(d => d.Approval == null)
                .ToAsyncEnumerable()
            ),
            cancellationToken,
            async (data, cancellationToken) =>
            {
                if ((await CreateResponseApprovalAsync(
                        data,
                        CreateResponseApprovalsErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                        responseApprovalService,
                        context,
                        cancellationToken
                    )
                    ).Failed(out var createResponseApprovalErrorPayload)
                )
                {
                    logger.FailedCreatingResponseApproval(data.GetType(), data.Id, createResponseApprovalErrorPayload.Errors?.Select(_ => _.Message));
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