using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;

namespace Database.GraphQl.ResponseApprovals;

[ExtendObjectType(nameof(Mutation))]
public sealed class ResponseApprovalMutations
{
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<CreateResponseApprovalsPayload> CreateResponseApprovalsAsync(
        ApplicationDbContext context,
        UserService userService,
        DatabaseService databaseService,
        ResponseApprovalService responseApprovalService,
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
        var database = await databaseService.QueryDatabase(cancellationToken);
        if (database is null)
        {
            return new CreateResponseApprovalsPayload(
                new CreateResponseApprovalsError(
                    CreateResponseApprovalsErrorCode.UNKNOWN_DATABASE,
                    $"The database could not be identified.",
                    []
                )
            );
        }
        if (!ResponseApprovalAuthorization.IsAuthorizedToManageResponseApprovals(
            currentUser,
            database.Operator.Node.Uuid
            )
        )
        {
            return new CreateResponseApprovalsPayload(
                new CreateResponseApprovalsError(
                    CreateResponseApprovalsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create response approvals for the database.",
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
                    errors.Add(
                        new CreateResponseApprovalsError(
                            CreateResponseApprovalsErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                            $"Creating response approval for data {data} failed with message: {exception.Message}",
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
        DatabaseService databaseService,
        ResponseApprovalService responseApprovalService,
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
        var database = await databaseService.QueryDatabase(cancellationToken);
        if (database is null)
        {
            return new UpdateResponseApprovalsPayload(
                new UpdateResponseApprovalsError(
                    UpdateResponseApprovalsErrorCode.UNKNOWN_DATABASE,
                    $"The database could not be identified.",
                    []
                )
            );
        }
        if (!ResponseApprovalAuthorization.IsAuthorizedToManageResponseApprovals(
            currentUser,
            database.Operator.Node.Uuid
            )
        )
        {
            return new UpdateResponseApprovalsPayload(
                new UpdateResponseApprovalsError(
                    UpdateResponseApprovalsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create response approvals for the database.",
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
                    errors.Add(
                        new UpdateResponseApprovalsError(
                            UpdateResponseApprovalsErrorCode.UPDATING_RESPONSE_APPROVAL_FAILED,
                            $"Updating response approval for data {data} failed with message: {exception.Message}",
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