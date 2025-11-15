using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Client.Abstractions.Utilities;
using Database.Data;
using Database.Enumerations;
using Database.Services;
using System.Collections.Generic;
using static Database.ApiRequests.DataFormatDataLoader;
using Database.ApiRequests;
using Database.Extensions;
using System.Linq;
using System.Globalization;

namespace Database.GraphQl;

public interface IIdentifyDataInput
{
    public Guid DataId { get; }
    public DataKind DataKind { get; }
}

public abstract class DataMutationBase<TData, TPayload, TError, TErrorCode>
: MutationBase<TData, TPayload, TError, TErrorCode>
where TData : class
where TPayload : class
where TError : class
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
        IData data,
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
                    null,
                    [NewError(
                        creatingResponseApprovalFailedErrorCode,
                        $"Failed creating a response approval for the data set {data.Id} named '{data.Name}' with the error message: {exception.Message}",
                        []
                    )]
                )
            );
        }
        return new Result<bool, TPayload>.Data(true);
    }

    protected async Task<Result<DataFormat, IReadOnlyList<TError>>> ValidateGetHttpsResourceAsync(
        IValidateGetHttpsResourceInput input,
        IReadOnlyList<string> getHttpsResourcePath,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        TErrorCode unknownDataFormatErrorCode,
        CancellationToken cancellationToken
    )
    {
        var validateDataFormats = new Task<Result<DataFormat, TError>>[1 + input.ArchivedFilesMetaInformation.Count];
        validateDataFormats[0] = Task.Run<Result<DataFormat, TError>>(async () =>
            {
                var dataFormat = await dataFormatByIdDataLoader.LoadAsync(input.DataFormatId, cancellationToken);
                if (dataFormat is null)
                {
                    return new Result<DataFormat, TError>.Error(
                            NewError(
                                unknownDataFormatErrorCode,
                                "The data format does not exist",
                                [.. getHttpsResourcePath, nameof(input.DataFormatId).ToLowerFirst()]
                                )
                            );
                }
                return new Result<DataFormat, TError>.Data(dataFormat);
            },
            cancellationToken
        );
        foreach (var (archivedFileMetaInformation, index) in input.ArchivedFilesMetaInformation.WithIndex())
        {
            validateDataFormats[index + 1] = Task.Run<Result<DataFormat, TError>>(async () =>
                {
                    var dataFormat = await dataFormatByIdDataLoader.LoadAsync(archivedFileMetaInformation.DataFormatId, cancellationToken);
                    if (dataFormat is null)
                    {
                        // TODO Is the index in the path formated correctly or should it be enclosed in square brackets?
                        return new Result<DataFormat, TError>.Error(
                                NewError(
                                    unknownDataFormatErrorCode,
                                    "The data format does not exist",
                                    [.. getHttpsResourcePath, nameof(input.ArchivedFilesMetaInformation).ToLowerFirst(), index.ToString(CultureInfo.InvariantCulture), nameof(archivedFileMetaInformation.DataFormatId).ToLowerFirst()]
                                    )
                                );
                    }
                    return new Result<DataFormat, TError>.Data(dataFormat);
                },
                cancellationToken
            );
        }
        var maybeDataFormats = await Task.WhenAll(validateDataFormats);
        var errors = maybeDataFormats
                .Select(_ => _.Failed(out var error) ? error : null)
                .NotNull()
                .ToList()
                .AsReadOnly();
        if (errors.Count >= 1)
        {
            return new Result<DataFormat, IReadOnlyList<TError>>.Error(errors);
        }
        maybeDataFormats[0].Succeeded(out var dataFormat);
        return new Result<DataFormat, IReadOnlyList<TError>>.Data(dataFormat ?? throw new InvalidOperationException("Impossible because there are no errors when this code line is being executed!"));
    }
}