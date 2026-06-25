using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;
using Database.Services;
using HotChocolate.Types;
using HotChocolate.Authorization;

namespace Database.GraphQl.DataX;

public sealed record RetractDataInput(
    Guid DataId,
    DataKind DataKind
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum RetractDataErrorCode
{
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    CREATING_RESPONSE_APPROVAL_FAILED,
    NOT_PUBLISHED
}

public sealed record RetractDataError(
    RetractDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
) : UserErrorBase<RetractDataErrorCode>(Code, Message, Path);

public sealed record RetractDataPayload(
    IData? Data,
    IReadOnlyCollection<RetractDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class RetractDataMutation
: DataMutationBase<IData, RetractDataPayload, RetractDataError, RetractDataErrorCode>
{
    protected override RetractDataPayload NewPayload(
        IData? data,
        IReadOnlyCollection<RetractDataError>? errors
    ) => new(data, errors);

    protected override RetractDataError NewError(
        RetractDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<RetractDataPayload> RetractDataAsync(
        RetractDataInput input,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                RetractDataErrorCode.UNAUTHENTICATED,
                RetractDataErrorCode.UNAUTHORIZED,
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
                RetractDataErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        if (data.PublishingState is not PublishingState.PUBLISHED)
        {
            return NewPayload(
                null,
                [NewError(
                    RetractDataErrorCode.NOT_PUBLISHED,
                    $"The publishing state is not published but {data.PublishingState}. If it is pending, you may delete the data set instead.",
                    []
                )]
            );
        }

        var rememberedPublishingState = data.PublishingState;
        data.Retract();
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                data,
                RetractDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            if (rememberedPublishingState is PublishingState.PUBLISHED)
            {
                data.Publish();
            }
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(data, null);
    }
}
