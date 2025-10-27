using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;

namespace Database.GraphQl.ResponseApprovals;

public static partial class ResponseApprovalMutationsLogging
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Creating response approval for data of type {Type} with ID {Id} failed."
    )]
    public static partial void FailedCreatingResponseApproval(this ILogger<ResponseApprovalMutations> logger, Type type, Guid id, Exception exception);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Updating response approval for data of type {Type} with ID {Id} failed."
    )]
    public static partial void FailedUpdatingResponseApproval(this ILogger<ResponseApprovalMutations> logger, Type type, Guid id, Exception exception);
}

[ExtendObjectType(nameof(Mutation))]
public sealed class ResponseApprovalMutations
{
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<CreateResponseApprovalsPayload> CreateResponseApprovalsAsync(
        ApplicationDbContext context,
        UserService userService,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ILogger<ResponseApprovalMutations> logger,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new CreateResponseApprovalsPayload(
                new CreateResponseApprovalsError(
                    CreateResponseApprovalsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreateResponseApprovalsPayload(
                new CreateResponseApprovalsError(
                    CreateResponseApprovalsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create response approvals in this database.",
                    []
                )
            );
        }
        var dataSets = new ConcurrentBag<IData>();
        var errors = new ConcurrentBag<CreateResponseApprovalsError>();
        await Parallel.ForEachAsync(
            context.CalorimetricData.AsQueryable<IData>().Where(d => d.Approval == null).ToAsyncEnumerable()
            .Union(context.GeometricData.AsQueryable<IData>().Where(d => d.Approval == null).ToAsyncEnumerable())
            .Union(context.HygrothermalData.AsQueryable<IData>().Where(d => d.Approval == null).ToAsyncEnumerable())
            .Union(context.OpticalData.AsQueryable<IData>().Where(d => d.Approval == null).ToAsyncEnumerable())
            .Union(context.PhotovoltaicData.AsQueryable<IData>().Where(d => d.Approval == null).ToAsyncEnumerable()),
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
                    logger.FailedCreatingResponseApproval(data.GetType(), data.Id, exception);
                    errors.Add(
                        new CreateResponseApprovalsError(
                            CreateResponseApprovalsErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                            $"Creating response approval for data `{data}` with ID {data.Id} failed with message: {exception.Message}",
                            []
                        )
                    );
                }
            }
        );
        await context.SaveChangesAsync(cancellationToken);
        if (!errors.IsEmpty)
        {
            return new CreateResponseApprovalsPayload(dataSets, errors);
        }
        return new CreateResponseApprovalsPayload(dataSets);
    }

    public async Task<UpdateResponseApprovalsPayload> UpdateResponseApprovalsAsync(
        ApplicationDbContext context,
        UserService userService,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ILogger<ResponseApprovalMutations> logger,
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
                    $"The current user is not authorized to create response approvals in this database.",
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