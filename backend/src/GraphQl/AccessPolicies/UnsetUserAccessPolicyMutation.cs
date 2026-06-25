using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record UnsetUserAccessPolicyInput
(
    DataReferenceInput? Data,
    Guid UserId
);

[SuppressMessage("Naming", "CA1707")]
public enum UnsetUserAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_POLICY
}

public sealed record UnsetUserAccessPolicyError(
    UnsetUserAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UnsetUserAccessPolicyErrorCode>(Code, Message, Path);

public sealed record UnsetUserAccessPolicyPayload(
    UserAccessPolicy? UserAccessPolicy,
    IReadOnlyCollection<UnsetUserAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class UnsetUserAccessPolicyMutation
: DataMutationBase<UserAccessPolicy, UnsetUserAccessPolicyPayload, UnsetUserAccessPolicyError, UnsetUserAccessPolicyErrorCode>
{
    protected override UnsetUserAccessPolicyPayload NewPayload(
        UserAccessPolicy? data,
        IReadOnlyCollection<UnsetUserAccessPolicyError>? errors
    ) => new(data, errors);

    protected override UnsetUserAccessPolicyError NewError(
        UnsetUserAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<UnsetUserAccessPolicyPayload> UnsetUserAccessPolicyAsync(
        UnsetUserAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IUserByIdDataLoader userByIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UnsetUserAccessPolicyErrorCode.UNAUTHENTICATED,
                UnsetUserAccessPolicyErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if (input.Data is not null)
        {
            if ((await FetchDataAsync(
                    input.Data,
                    UnsetUserAccessPolicyErrorCode.UNKNOWN_DATA,
                    context,
                    cancellationToken
                )
                ).Failed(out var _, out var fetchDataErrorPayload)
            )
            {
                return fetchDataErrorPayload;
            }
        }

        var dataId = input.Data?.DataId;
        var userAccessPolicy = await context.UserAccessPolicies
            .SingleOrDefaultAsync(_ =>
                _.DataAccessPolicy != null
                && _.DataAccessPolicy.DataId == dataId
                && _.UserId == input.UserId,
                cancellationToken
            );
        if (userAccessPolicy is null)
        {
            return NewPayload(
                null,
                [NewError(
                    UnsetUserAccessPolicyErrorCode.UNKNOWN_POLICY,
                    $"The user access policy does not exist.",
                    [nameof(input), nameof(input.UserId).ToLowerFirst()]
                )]
            );
        }

        context.UserAccessPolicies.Remove(userAccessPolicy);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(userAccessPolicy, null);
    }
}
