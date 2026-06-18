using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;
using HotChocolate.Types;
using HotChocolate.Authorization;

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
    UNKNOWN_DATA,
    NOT_PENDING
}

public sealed record DeleteDataError(
    DeleteDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
) : UserErrorBase<DeleteDataErrorCode>(Code, Message, Path);

public sealed record DeleteDataPayload(
    // [GraphQLType<ListType<ErrorType<DeleteDataErrorCode>>>]
    IReadOnlyCollection<DeleteDataError>? Errors
) : Payload;

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
    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
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

        if (data.PublishingState is not PublishingState.PENDING)
        {
            return NewPayload(
                null,
                [NewError(
                    DeleteDataErrorCode.NOT_PENDING,
                    $"The publishing state is not pending but {data.PublishingState}. If it is published, you may retract the data set. A retracted data set does not show up in queries for all data.",
                    []
                )]
            );
        }

        // Delete resource files if they exist.
        // The resources themselves are deleted through cascading deletes.
        await context.Entry(data)
            .Collection(_ => _.Resources)
            .LoadAsync(cancellationToken);
        foreach (var resource in data.Resources)
        {
            resource.DeleteFile();
        }

        context.Remove(data);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data, null);
    }
}
