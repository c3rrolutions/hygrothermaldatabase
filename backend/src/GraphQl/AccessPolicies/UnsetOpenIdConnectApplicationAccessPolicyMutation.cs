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

public sealed record UnsetOpenIdConnectApplicationAccessPolicyInput
(
    DataReferenceInput? Data,
    string ClientId
);

[SuppressMessage("Naming", "CA1707")]
public enum UnsetOpenIdConnectApplicationAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_POLICY
}

public sealed record UnsetOpenIdConnectApplicationAccessPolicyError(
    UnsetOpenIdConnectApplicationAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UnsetOpenIdConnectApplicationAccessPolicyErrorCode>(Code, Message, Path);

public sealed record UnsetOpenIdConnectApplicationAccessPolicyPayload(
    OpenIdConnectApplicationAccessPolicy? ApplicationAccessPolicy,
    IReadOnlyCollection<UnsetOpenIdConnectApplicationAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class UnsetOpenIdConnectApplicationAccessPolicyMutation
: DataMutationBase<OpenIdConnectApplicationAccessPolicy, UnsetOpenIdConnectApplicationAccessPolicyPayload, UnsetOpenIdConnectApplicationAccessPolicyError, UnsetOpenIdConnectApplicationAccessPolicyErrorCode>
{
    protected override UnsetOpenIdConnectApplicationAccessPolicyPayload NewPayload(
        OpenIdConnectApplicationAccessPolicy? data,
        IReadOnlyCollection<UnsetOpenIdConnectApplicationAccessPolicyError>? errors
    ) => new(data, errors);

    protected override UnsetOpenIdConnectApplicationAccessPolicyError NewError(
        UnsetOpenIdConnectApplicationAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<UnsetOpenIdConnectApplicationAccessPolicyPayload> UnsetOpenIdConnectApplicationAccessPolicyAsync(
        UnsetOpenIdConnectApplicationAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IOpenIdConnectApplicationByClientIdDataLoader openIdConnectApplicationByClientIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UnsetOpenIdConnectApplicationAccessPolicyErrorCode.UNAUTHENTICATED,
                UnsetOpenIdConnectApplicationAccessPolicyErrorCode.UNAUTHORIZED,
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
                    UnsetOpenIdConnectApplicationAccessPolicyErrorCode.UNKNOWN_DATA,
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
        var applicationAccessPolicy = await context.OpenIdConnectApplicationAccessPolicies
            .SingleOrDefaultAsync(_ =>
                _.DataAccessPolicy != null
                && _.DataAccessPolicy.DataId == dataId
                && _.ClientId == input.ClientId,
                cancellationToken
            );
        if (applicationAccessPolicy is null)
        {
            return NewPayload(
                null,
                [NewError(
                    UnsetOpenIdConnectApplicationAccessPolicyErrorCode.UNKNOWN_POLICY,
                    $"The Open ID Connect application access policy does not exist.",
                    [nameof(input), nameof(input.ClientId).ToLowerFirst()]
                )]
            );
        }

        context.OpenIdConnectApplicationAccessPolicies.Remove(applicationAccessPolicy);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(applicationAccessPolicy, null);
    }
}
