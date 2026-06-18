using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.Scalars;
using Database.Enumerations;
using Database.Extensions;
using Database.Services;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;

namespace Database.GraphQl.DataX;

public sealed record UpdateDataInput(
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid DataId,
    DataKind DataKind,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTimeOffset CreatedAt,
    Guid CreatorId
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateDataErrorCode
{
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record UpdateDataError(
    UpdateDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
) : UserErrorBase<UpdateDataErrorCode>(Code, Message, Path);

public sealed record UpdateDataPayload(
    IData? Data,
    IReadOnlyCollection<UpdateDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateDataMutation
: DataMutationBase<IData, UpdateDataPayload, UpdateDataError, UpdateDataErrorCode>
{
    protected override UpdateDataPayload NewPayload(
        IData? data,
        IReadOnlyCollection<UpdateDataError>? errors
    ) => new(data, errors);

    protected override UpdateDataError NewError(
        UpdateDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<UpdateDataPayload> UpdateDataAsync(
        UpdateDataInput input,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UpdateDataErrorCode.UNAUTHENTICATED,
                UpdateDataErrorCode.UNAUTHORIZED,
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
                UpdateDataErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        var rememberedValues = new UpdateDataInput(
            data.Locale,
            input.DataId,
            input.DataKind,
            data.ComponentId,
            data.Name,
            data.Description,
            data.Warnings,
            data.CreatedAt,
            data.CreatorId
        );
        data.Update(
            input.Locale,
            input.ComponentId,
            input.Name,
            input.Description,
            input.Warnings,
            input.CreatedAt,
            input.CreatorId
        );
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                data,
                UpdateDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            data.Update(
                rememberedValues.Locale,
                rememberedValues.ComponentId,
                rememberedValues.Name,
                rememberedValues.Description,
                rememberedValues.Warnings,
                rememberedValues.CreatedAt,
                rememberedValues.CreatorId
            );
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(data, null);
    }
}
