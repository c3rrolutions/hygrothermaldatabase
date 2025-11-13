using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Database.ApiRequests;
using GraphQL.Client.Abstractions.Utilities;
using Database.Extensions;
using System.Globalization;
using static Database.ApiRequests.DataFormatDataLoader;
using System.Linq;

namespace Database.GraphQl.DataX;

public interface IValidateCreateInput
{
    Guid ComponentId { get; }
    DateTime CreatedAt { get; }
    Guid CreatorId { get; }
    AppliedMethodInput AppliedMethod { get; }
    RootGetHttpsResourceInput RootResource { get; }
}

public abstract class CreateDataMutationBase<TData, TPayload, TError, TErrorCode>
: DataMutationBase<TData, TPayload, TError, TErrorCode>
where TData : class
where TPayload : class
where TError : class
{
    protected async Task<Result<DataFormat, TPayload>> ValidateAsync(
        IValidateCreateInput input,
        TErrorCode illegalCreatedAtErrorCode,
        IComponentByIdDataLoader componentByIdDataLoader,
        TErrorCode unknownComponentErrorCode,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        TErrorCode unknownCreatorErrorCode,
        IMethodByIdDataLoader methodByIdDataLoader,
        TErrorCode unknownAppliedMethodErrorCode,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        TErrorCode unknownDataFormatErrorCode,
        CancellationToken cancellationToken
    )
    {
        var errors = new List<TError>();
        if (await componentByIdDataLoader.LoadAsync(input.ComponentId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    unknownComponentErrorCode,
                    "The data format does not exist.",
                    [nameof(input), nameof(input.ComponentId).ToLowerFirst()]
                )
            );
        }
        if (input.CreatedAt > DateTime.UtcNow)
        {
            errors.Add(
                NewError(
                    illegalCreatedAtErrorCode,
                    "The creation date is in the future.",
                    [nameof(input), nameof(input.CreatedAt).ToLowerFirst()]
                )
            );
        }
        if (await institutionByIdDataLoader.LoadAsync(input.CreatorId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    unknownCreatorErrorCode,
                    "The creator does not exist.",
                    [nameof(input), nameof(input.CreatorId).ToLowerFirst()]
                )
            );
        }
        if (await methodByIdDataLoader.LoadAsync(input.AppliedMethod.MethodId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    unknownAppliedMethodErrorCode,
                    "The applied method does not exist",
                    [nameof(input), nameof(input.AppliedMethod).ToLowerFirst(), nameof(input.AppliedMethod.MethodId).ToLowerFirst()]
                )
            );
        }
        // TODO check applied method sources
        var validateDataFormats = new Task<Result<DataFormat, TError>>[1 + input.RootResource.ArchivedFilesMetaInformation.Count];
        validateDataFormats[0] = Task.Run<Result<DataFormat, TError>>(async () =>
            {
                var dataFormat = await dataFormatByIdDataLoader.LoadAsync(input.RootResource.DataFormatId, cancellationToken);
                if (dataFormat is null)
                {
                    return new Result<DataFormat, TError>.Error(
                        NewError(
                            unknownDataFormatErrorCode,
                            "The data format does not exist",
                            [nameof(input), nameof(input.RootResource).ToLowerFirst(), nameof(input.RootResource.DataFormatId).ToLowerFirst()]
                        )
                    );
                }
                return new Result<DataFormat, TError>.Data(dataFormat);
            },
            cancellationToken
        );
        foreach (var (archivedFileMetaInformation, index) in input.RootResource.ArchivedFilesMetaInformation.WithIndex())
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
                                [nameof(input), nameof(input.RootResource).ToLowerFirst(), nameof(input.RootResource.ArchivedFilesMetaInformation).ToLowerFirst(), index.ToString(CultureInfo.InvariantCulture), nameof(archivedFileMetaInformation.DataFormatId).ToLowerFirst()]
                            )
                        );
                    }
                    return new Result<DataFormat, TError>.Data(dataFormat);
                },
                cancellationToken
            );
        }
        var maybeDataFormats = await Task.WhenAll(validateDataFormats);
        errors.AddRange(
            maybeDataFormats
            .Select(_ => _.Failed(out var error) ? error : null)
            .NotNull()
        );
        if (errors.Count >= 1)
        {
            return new Result<DataFormat, TPayload>.Error(
                NewPayload(null, errors)
            );
        }
        maybeDataFormats[0].Succeeded(out var dataFormat);
        return new Result<DataFormat, TPayload>.Data(dataFormat ?? throw new InvalidOperationException("Impossible because there are no errors when this code line is being executed!"));
    }
}