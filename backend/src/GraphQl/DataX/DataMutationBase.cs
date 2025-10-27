using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Client.Abstractions.Utilities;
using Database.Data;
using Database.Enumerations;
using Database.Services;

namespace Database.GraphQl.DataX;

public interface IIdentifyDataInput
{
    public Guid DataId { get; }
    public DataKind DataKind { get; }
}

public abstract class DataMutationBase<TData, TPayload, TError, TErrorCode>
: MutationBase<TData, TPayload, TError, TErrorCode>
where TData : class, IData
where TPayload : class
{
    protected async Task<Result<IData, TPayload>> FetchDataAsync(
        IIdentifyDataInput input,
        TErrorCode unknownDataErrorCode,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var data = await context.GetDataAsync(input.DataId, input.DataKind, cancellationToken);
        if (data is null)
        {
            return new Result<IData, TPayload>.Error(
                NewPayload(null, [
                    NewError(
                        unknownDataErrorCode,
                        $"Data with ID '{input.DataId}' and kind '{input.DataKind}' was not found.",
                        [nameof(input), nameof(input.DataId).ToLowerFirst()]
                    )
                ])
            );
        }
        return new Result<IData, TPayload>.Data(data);
    }

    protected async Task<Result<bool, TPayload>> CreateResponseApprovalAsync(
        TData data,
        TErrorCode creatingResponseApprovalFailedErrorCode,
        ResponseApprovalService responseApprovalService,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        try
        {
            data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            return new Result<bool, TPayload>.Error(
                NewPayload(
                    data,
                    [NewError(
                        creatingResponseApprovalFailedErrorCode,
                        $"Signing failed with message: {exception.Message}",
                        []
                    )]
                )
            );
        }
        return new Result<bool, TPayload>.Data(true);
    }
}