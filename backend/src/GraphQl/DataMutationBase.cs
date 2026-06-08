using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.Enumerations;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using static Database.ApiRequests.DataFormatDataLoader;

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
        var validateDataFormats = new Task<DataFormat?>[1 + input.ArchivedFilesMetaInformation.Count];
        // First eagerly execute all `LoadAsync`s on data loaders to make it know all keys.
        validateDataFormats[0] = dataFormatByIdDataLoader.LoadAsync(input.DataFormatId, cancellationToken);
        foreach (var (index, archivedFileMetaInformation) in input.ArchivedFilesMetaInformation.Index())
        {
            validateDataFormats[index + 1] = dataFormatByIdDataLoader.LoadAsync(archivedFileMetaInformation.DataFormatId, cancellationToken);
        }
        // Then await all "loads".
        var dataFormats = await Task.WhenAll(validateDataFormats);
        // Finally collect errors.
        var errors = new List<TError>();
        var dataFormat = dataFormats[0];
        if (dataFormat is null)
        {
            errors.Add(
                NewError(
                    unknownDataFormatErrorCode,
                    "The data format does not exist",
                    [.. getHttpsResourcePath, nameof(input.DataFormatId).ToLowerFirst()]
                )
            );
        }
        foreach (var (index, archivedFileDataFormat) in dataFormats[1..].Index())
        {
            if (archivedFileDataFormat is null)
            {
                // TODO Is the index in the path formated correctly or should it be enclosed in square brackets?
                errors.Add(
                    NewError(
                        unknownDataFormatErrorCode,
                        "The data format does not exist",
                        [.. getHttpsResourcePath, nameof(input.ArchivedFilesMetaInformation).ToLowerFirst(), index.ToString(CultureInfo.InvariantCulture), nameof(FileMetaInformationInput.DataFormatId).ToLowerFirst()]
                    )
                );
            }
        }
        if (errors.Count > 0)
        {
            return new Result<DataFormat, IReadOnlyList<TError>>.Error(errors);
        }
        return new Result<DataFormat, IReadOnlyList<TError>>.Data(dataFormat ?? throw new InvalidOperationException("Impossible because there are no errors when this code line is being executed!"));
    }
}