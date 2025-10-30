using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;
using Database.Services;

namespace Database.GraphQl.DataX;

public sealed record PublishDataInput(
    Guid DataId,
    DataKind DataKind
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum PublishDataErrorCode
{
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record PublishDataError(
    PublishDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
) : UserErrorBase<PublishDataErrorCode>(Code, Message, Path);

public sealed record PublishDataPayload(
    IData? Data,
    IReadOnlyCollection<PublishDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class PublishDataMutation
: DataMutationBase<IData, PublishDataPayload, PublishDataError, PublishDataErrorCode>
{
    protected override PublishDataPayload NewPayload(
        IData? data,
        IReadOnlyCollection<PublishDataError>? errors
    ) => new(data, errors);

    protected override PublishDataError NewError(
        PublishDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<PublishDataPayload> PublishDataAsync(
        PublishDataInput input,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                PublishDataErrorCode.UNAUTHENTICATED,
                PublishDataErrorCode.UNAUTHORIZED,
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
                PublishDataErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        var rememberedPublishingState = data.PublishingState;
        data.Publish();
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                data,
                PublishDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            if (rememberedPublishingState is PublishingState.RETRACTED)
            {
                data.Retract();
            }
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(data, null);
    }
}