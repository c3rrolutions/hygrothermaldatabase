using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Enumerations;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;

namespace Database.GraphQl.DataX;

public abstract class DataMutationsBase
: MutationsBase
{
    public async Task<TPayload> ActAndThenCreateResponseApprovalAsync<TPayload, TData, TError, TErrorCode>(
        TErrorCode creatingResponseApprovalFailedErrorCode,
        Func<TData?, IReadOnlyCollection<TError>?, TPayload> newPayload,
        Func<TErrorCode, string, IReadOnlyList<string>, TError> newError,
        ApplicationDbContext context,
        ResponseApprovalService responseApprovalService,
        Func<Task<TData>> act,
        Func<TData, Task> undo,
        CancellationToken cancellationToken
    ) where TData : IData
    {
        var data = await act();
        try
        {
            data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            await undo(data);
            return newPayload(
                data,
                [newError(
                    creatingResponseApprovalFailedErrorCode,
                    $"Signing failed with message: {exception.Message}",
                    []
                )]
            );
        }
        return newPayload(data, null);
    }

    protected interface IChangeDataInput
    {
        public Guid DataId { get; }
        public DataKind DataKind { get; }
    }

    protected static async Task<TPayload> FetchDataAsync<TPayload, TError, TErrorCode>(
        IChangeDataInput input,
        TErrorCode unknownDataErrorCode,
        Func<IData?, IReadOnlyCollection<TError>?, TPayload> newPayload,
        Func<TErrorCode, string, IReadOnlyList<string>, TError> newError,
        ApplicationDbContext context,
        Func<IData, Task<TPayload>> then,
        CancellationToken cancellationToken
    )
    {
        var data = await context.GetDataAsync(input.DataId, input.DataKind, cancellationToken);
        if (data is null)
        {
            return newPayload(null, [
                newError(
                    unknownDataErrorCode,
                    $"Data with ID '{input.DataId}' and kind '{input.DataKind}' was not found.",
                    [nameof(input), nameof(input.DataId).ToLowerFirst()]
                )
            ]);
        }
        return await then(data);
    }
}