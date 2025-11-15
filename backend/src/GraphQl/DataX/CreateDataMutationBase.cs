using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Database.ApiRequests;
using GraphQL.Client.Abstractions.Utilities;
using Database.Extensions;
using static Database.ApiRequests.DataFormatDataLoader;

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
        var validateRootResourceResult = await ValidateGetHttpsResourceAsync(
            input.RootResource,
            [nameof(input), nameof(input.RootResource)],
            dataFormatByIdDataLoader,
            unknownDataFormatErrorCode,
            cancellationToken
        );
        if (validateRootResourceResult.Failed(out var dataFormat, out var validateRootResourceErrors))
        {
            errors.AddRange(validateRootResourceErrors);
        }
        // Note that `dataFormat` is only `null`, when `validateRootResourceResult` failed.
        if (errors.Count >= 1 || dataFormat is null)
        {
            return new Result<DataFormat, TPayload>.Error(
                NewPayload(null, errors)
            );
        }
        return new Result<DataFormat, TPayload>.Data(dataFormat);
    }
}