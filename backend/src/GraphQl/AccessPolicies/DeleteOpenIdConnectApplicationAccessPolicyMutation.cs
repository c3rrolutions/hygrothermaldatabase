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

public sealed record DeleteOpenIdConnectApplicationAccessPolicyInput
(
    string ClientId
);

[SuppressMessage("Naming", "CA1707")]
public enum DeleteOpenIdConnectApplicationAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_POLICY
}

public sealed record DeleteOpenIdConnectApplicationAccessPolicyError(
    DeleteOpenIdConnectApplicationAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<DeleteOpenIdConnectApplicationAccessPolicyErrorCode>(Code, Message, Path);

public sealed record DeleteOpenIdConnectApplicationAccessPolicyPayload(
   OpenIdConnectApplicationAccessPolicy? OpenIdConnectApplicationAccessPolicy,
   IReadOnlyCollection<DeleteOpenIdConnectApplicationAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteOpenIdConnectApplicationAccessPolicyMutation
: MutationBase<OpenIdConnectApplicationAccessPolicy, DeleteOpenIdConnectApplicationAccessPolicyPayload, DeleteOpenIdConnectApplicationAccessPolicyError, DeleteOpenIdConnectApplicationAccessPolicyErrorCode>
{
    protected override DeleteOpenIdConnectApplicationAccessPolicyPayload NewPayload(
        OpenIdConnectApplicationAccessPolicy? data,
        IReadOnlyCollection<DeleteOpenIdConnectApplicationAccessPolicyError>? errors
    ) => new(data, errors);

    protected override DeleteOpenIdConnectApplicationAccessPolicyError NewError(
        DeleteOpenIdConnectApplicationAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<DeleteOpenIdConnectApplicationAccessPolicyPayload> DeleteOpenIdConnectApplicationAccessPolicyAsync(
        DeleteOpenIdConnectApplicationAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteOpenIdConnectApplicationAccessPolicyErrorCode.UNAUTHENTICATED,
                DeleteOpenIdConnectApplicationAccessPolicyErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var accessPolicy = await context.OpenIdConnectApplicationAccessPolicies
            .SingleOrDefaultAsync(x =>
                x.ClientId == input.ClientId,
                cancellationToken
            );
        if (accessPolicy is null)
        {
            return NewPayload(
                null,
                [NewError(
                    DeleteOpenIdConnectApplicationAccessPolicyErrorCode.UNKNOWN_POLICY,
                    $"The access policy does not exist.",
                    [nameof(input), nameof(input.ClientId).ToLowerFirst()]
                )]
            );
        }
        context.OpenIdConnectApplicationAccessPolicies.Remove(accessPolicy);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicy, null);
    }
}