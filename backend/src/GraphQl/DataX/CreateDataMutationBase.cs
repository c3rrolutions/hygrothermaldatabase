using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Extensions;
using GraphQL.Client.Abstractions.Utilities;
using NodaTime;
using static Database.ApiRequests.DataFormatDataLoader;

namespace Database.GraphQl.DataX;

public interface IValidateCreateInput
{
    Guid ComponentId { get; }
    DateTimeOffset CreatedAt { get; }
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
        IDataByDatabaseAndIdAndKindDataLoader dataByDatabaseAndIdAndKindDataLoader,
        TErrorCode unknownDatabase,
        TErrorCode unknownCrossDatabaseData,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        TErrorCode unknownDataFormatErrorCode,
        IClock clock,
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
        if (input.CreatedAt > clock.GetUtcNow().ToDateTimeOffset())
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
                    "The applied method does not exist.",
                    [nameof(input), nameof(input.AppliedMethod).ToLowerFirst(), nameof(input.AppliedMethod.MethodId).ToLowerFirst()]
                )
            );
        }
        // TODO check applied method argument and source names
        var databases = await Task.WhenAll(
            input.AppliedMethod.Sources.Select(source =>
                dataByDatabaseAndIdAndKindDataLoader.LoadAsync(
                    (source.Value.DatabaseId, source.Value.DataId, source.Value.DataKind),
                    cancellationToken
                )
            )
            .ToImmutableArray() // Execute eagerly to have the data loader collect all keys first.
        );
        foreach (var (index, database) in databases.Index())
        {
            string[] valuePath = [
                nameof(input),
                nameof(input.AppliedMethod).ToLowerFirst(),
                nameof(input.AppliedMethod.Sources).ToLowerFirst(),
                index.ToString(CultureInfo.InvariantCulture),
                nameof(NamedMethodSourceInput.Value)
            ];
            if (database is null)
            {
                errors.Add(
                    NewError(
                        unknownDatabase,
                        "The database does not exist.",
                        [
                            ..valuePath,
                            nameof(NamedMethodSourceInput.Value.DatabaseId),
                        ]
                    )
                );
            }
            else if (database.Data is null)
            {
                errors.Add(
                    NewError(
                        unknownCrossDatabaseData,
                        "The data does not exist with the given kind.",
                        [
                            ..valuePath,
                            nameof(NamedMethodSourceInput.Value.DataId),
                        ]
                    )
                );
            }
        }
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
        if (errors.Count > 0 || dataFormat is null)
        {
            return new Result<DataFormat, TPayload>.Error(
                NewPayload(null, errors)
            );
        }
        return new Result<DataFormat, TPayload>.Data(dataFormat);
    }
}
