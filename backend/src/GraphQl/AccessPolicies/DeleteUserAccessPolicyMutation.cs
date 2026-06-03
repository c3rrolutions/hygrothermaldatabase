using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record DeleteUserAccessPolicyInput
(
    Guid UserId
);

[SuppressMessage("Naming", "CA1707")]
public enum DeleteUserAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_POLICY
}

public sealed record DeleteUserAccessPolicyError(
    DeleteUserAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<DeleteUserAccessPolicyErrorCode>(Code, Message, Path);

public sealed record DeleteUserAccessPolicyPayload(
   UserAccessPolicy? UserAccessPolicy,
   IReadOnlyCollection<DeleteUserAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteUserAccessPolicyMutation
: MutationBase<UserAccessPolicy, DeleteUserAccessPolicyPayload, DeleteUserAccessPolicyError, DeleteUserAccessPolicyErrorCode>
{
    protected override DeleteUserAccessPolicyPayload NewPayload(
        UserAccessPolicy? data,
        IReadOnlyCollection<DeleteUserAccessPolicyError>? errors
    ) => new(data, errors);

    protected override DeleteUserAccessPolicyError NewError(
        DeleteUserAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<DeleteUserAccessPolicyPayload> DeleteUserAccessPolicyAsync(
        DeleteUserAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteUserAccessPolicyErrorCode.UNAUTHENTICATED,
                DeleteUserAccessPolicyErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var accessPolicy = await context.UserAccessPolicies
            .SingleOrDefaultAsync(x =>
                x.UserId == input.UserId,
                cancellationToken
            );
        if (accessPolicy is null)
        {
            return NewPayload(
                null,
                [NewError(
                    DeleteUserAccessPolicyErrorCode.UNKNOWN_POLICY,
                    $"The access policy does not exist.",
                    [nameof(input), nameof(input.UserId).ToLowerFirst()]
                )]
            );
        }
        context.UserAccessPolicies.Remove(accessPolicy);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicy, null);
    }
}