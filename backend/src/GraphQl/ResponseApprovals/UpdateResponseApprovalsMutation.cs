using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;

namespace Database.GraphQl.ResponseApprovals;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateResponseApprovalsErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_DATABASE,
    UPDATING_RESPONSE_APPROVAL_FAILED
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

public sealed class UpdateResponseApprovalsPayload
    : Payload
{
    public IReadOnlyCollection<IData>? Data { get; }
    public IReadOnlyCollection<UpdateResponseApprovalsError>? Errors { get; }

    public UpdateResponseApprovalsPayload(
        IReadOnlyCollection<IData> data
    )
    {
        Data = data;
    }

    public UpdateResponseApprovalsPayload(
        UpdateResponseApprovalsError error
    )
    {
        Errors = [error];
    }

    public UpdateResponseApprovalsPayload(
        IReadOnlyCollection<IData> data,
        IReadOnlyCollection<UpdateResponseApprovalsError> errors
    ) : this(data)
    {
        Errors = errors;
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateResponseApprovalsMutation
{
    public async Task<UpdateResponseApprovalsPayload> UpdateResponseApprovalsAsync(
        ApplicationDbContext context,
        UserService userService,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ILogger<UpdateResponseApprovalsMutation> logger,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new UpdateResponseApprovalsPayload(
                new UpdateResponseApprovalsError(
                    UpdateResponseApprovalsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new UpdateResponseApprovalsPayload(
                new UpdateResponseApprovalsError(
                    UpdateResponseApprovalsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to update response approvals in this database.",
                    []
                )
            );
        }
        var dataSets = new ConcurrentBag<IData>();
        var errors = new ConcurrentBag<UpdateResponseApprovalsError>();
        await Parallel.ForEachAsync(
            context.CalorimetricData.AsQueryable<IData>().Where(d => d.Approval != null).ToAsyncEnumerable()
            .Union(context.GeometricData.AsQueryable<IData>().Where(d => d.Approval != null).ToAsyncEnumerable())
            .Union(context.HygrothermalData.AsQueryable<IData>().Where(d => d.Approval != null).ToAsyncEnumerable())
            .Union(context.OpticalData.AsQueryable<IData>().Where(d => d.Approval != null).ToAsyncEnumerable())
            .Union(context.PhotovoltaicData.AsQueryable<IData>().Where(d => d.Approval != null).ToAsyncEnumerable()),
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
                        new UpdateResponseApprovalsError(
                            UpdateResponseApprovalsErrorCode.UPDATING_RESPONSE_APPROVAL_FAILED,
                            $"Updating response approval for data `{data}` with ID {data.Id} failed with message: {exception.Message}",
                            []
                        )
                    );
                }
            }
        );
        await context.SaveChangesAsync(cancellationToken);
        if (!errors.IsEmpty)
        {
            return new UpdateResponseApprovalsPayload(dataSets, errors);
        }
        return new UpdateResponseApprovalsPayload(dataSets);
    }
}