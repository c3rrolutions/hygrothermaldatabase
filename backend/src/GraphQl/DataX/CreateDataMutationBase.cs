using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Database.ApiRequests;
using GraphQL.Client.Abstractions.Utilities;

namespace Database.GraphQl.DataX;

public interface IValidateCreateInput
{
    Guid ComponentId { get; }
    DateTime CreatedAt { get; }
    Guid CreatorId { get; }
    AppliedMethodInput AppliedMethod { get; }
}

public abstract class CreateDataMutationBase<TData, TPayload, TError, TErrorCode>
: DataMutationBase<TData, TPayload, TError, TErrorCode>
where TData : class
where TPayload : class
{
    protected async Task<Result<bool, TPayload>> ValidateAsync(
        IValidateCreateInput input,
        TErrorCode illegalCreatedAtErrorCode,
        IComponentByIdDataLoader componentByIdDataLoader,
        TErrorCode unknownComponentErrorCode,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        TErrorCode unknownCreatorErrorCode,
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
        // TODO check applied method sources
        if (errors.Count >= 1)
        {
            return new Result<bool, TPayload>.Error(
                NewPayload(null, errors)
            );
        }
        return new Result<bool, TPayload>.Data(true);
    }
}