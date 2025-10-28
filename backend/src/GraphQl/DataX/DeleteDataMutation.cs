using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;

namespace Database.GraphQl.DataX;

public sealed record DeleteDataInput(
    Guid DataId,
    DataKind DataKind
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum DeleteDataErrorCode
{
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record DeleteDataError(
    DeleteDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
) : UserErrorBase<DeleteDataErrorCode>(Code, Message, Path);

public sealed record DeleteDataPayload(
    // [GraphQLType<ListType<ErrorType<DeleteDataErrorCode>>>]
    IReadOnlyCollection<DeleteDataError>? Errors
);

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteDataMutation
: DataMutationBase<IData, DeleteDataPayload, DeleteDataError, DeleteDataErrorCode>
{
    protected override DeleteDataPayload NewPayload(
        IData? data,
        IReadOnlyCollection<DeleteDataError>? errors
    ) => new(errors);

    protected override DeleteDataError NewError(
        DeleteDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    // [GraphQLType<PayloadType<DeleteDataErrorCode>>]
    public async Task<DeleteDataPayload> DeleteDataAsync(
        DeleteDataInput input,
        CommonAuthorization authorization,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteDataErrorCode.UNAUTHENTICATED,
                DeleteDataErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if ((await FetchDataAsync(
                input,
                DeleteDataErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        switch (data)
        {
            case CalorimetricData calorimetricData:
                context.CalorimetricData.Remove(calorimetricData);
                break;
            case GeometricData geometricData:
                context.GeometricData.Remove(geometricData);
                break;
            case HygrothermalData hygrothermalData:
                context.HygrothermalData.Remove(hygrothermalData);
                break;
            case PhotovoltaicData photovoltaicData:
                context.PhotovoltaicData.Remove(photovoltaicData);
                break;
            case OpticalData opticalData:
                context.OpticalData.Remove(opticalData);
                break;
            default:
                throw new NotSupportedException($"The data kind '{input.DataKind}' is not supported for deletion.");
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data, null);
    }
}