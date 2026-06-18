using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using GraphQL.Client.Abstractions.Utilities;
using Database.ApiRequests;

namespace Database.GraphQl.AccessPolicies;

public sealed record SetOpenIdConnectApplicationAccessPolicyInput
(
    DataReferenceInput? Data,
    string ClientId,
    UpperLimitPerDurationInput? UpperAccessLimitPerTimeDuration
);

[SuppressMessage("Naming", "CA1707")]
public enum SetOpenIdConnectApplicationAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_APPLICATION
}

public sealed record SetOpenIdConnectApplicationAccessPolicyError(
    SetOpenIdConnectApplicationAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetOpenIdConnectApplicationAccessPolicyErrorCode>(Code, Message, Path);

public sealed record SetOpenIdConnectApplicationAccessPolicyPayload(
   OpenIdConnectApplicationAccessPolicy? OpenIdConnectApplicationAccessPolicy,
   IReadOnlyCollection<SetOpenIdConnectApplicationAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetOpenIdConnectApplicationAccessPolicyMutation
: DataMutationBase<OpenIdConnectApplicationAccessPolicy, SetOpenIdConnectApplicationAccessPolicyPayload, SetOpenIdConnectApplicationAccessPolicyError, SetOpenIdConnectApplicationAccessPolicyErrorCode>
{
    protected override SetOpenIdConnectApplicationAccessPolicyPayload NewPayload(
        OpenIdConnectApplicationAccessPolicy? data,
        IReadOnlyCollection<SetOpenIdConnectApplicationAccessPolicyError>? errors
    ) => new(data, errors);

    protected override SetOpenIdConnectApplicationAccessPolicyError NewError(
        SetOpenIdConnectApplicationAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<SetOpenIdConnectApplicationAccessPolicyPayload> SetOpenIdConnectApplicationAccessPolicyAsync(
        SetOpenIdConnectApplicationAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IOpenIdConnectApplicationByClientIdDataLoader applicationByClientIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetOpenIdConnectApplicationAccessPolicyErrorCode.UNAUTHENTICATED,
                SetOpenIdConnectApplicationAccessPolicyErrorCode.UNAUTHORIZED,
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
                    SetOpenIdConnectApplicationAccessPolicyErrorCode.UNKNOWN_DATA,
                    context,
                    cancellationToken
                )
                ).Failed(out var _, out var fetchDataErrorPayload)
            )
            {
                return fetchDataErrorPayload;
            }
        }

        List<SetOpenIdConnectApplicationAccessPolicyError> errors = [];
        if (await applicationByClientIdDataLoader.LoadAsync(input.ClientId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    SetOpenIdConnectApplicationAccessPolicyErrorCode.UNKNOWN_APPLICATION,
                    $"The Open ID Connect application does not exist.",
                    [nameof(input), nameof(input.ClientId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        var dataId = input.Data?.DataId;
        var dataAccessPolicy = await context.DataAccessPolicies
            .SingleAsync(_ => _.DataId == dataId, cancellationToken);
        var applicationAccessPolicy = await context.OpenIdConnectApplicationAccessPolicies
            .SingleOrDefaultAsync(_ =>
                _.DataAccessPolicyId == dataAccessPolicy.Id
                && _.ClientId == input.ClientId,
                cancellationToken
            );
        if (applicationAccessPolicy is null)
        {
            applicationAccessPolicy = new OpenIdConnectApplicationAccessPolicy(dataAccessPolicy.Id, input.ClientId)
            {
                UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel()
            };
            context.OpenIdConnectApplicationAccessPolicies.Add(applicationAccessPolicy);
        }
        else
        {
            applicationAccessPolicy.UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel();
            applicationAccessPolicy.AccessCountSinceStartTime = null;
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(applicationAccessPolicy, null);
    }
}
