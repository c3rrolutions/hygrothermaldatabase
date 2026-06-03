using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record SetOpenIdConnectApplicationAccessLimitInput
(
    string ClientId,
    UpperLimitPerDurationInput? UpperAccessLimitPerTimeDuration
);

[SuppressMessage("Naming", "CA1707")]
public enum SetOpenIdConnectApplicationAccessLimitErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED
}

public sealed record SetOpenIdConnectApplicationAccessLimitError(
    SetOpenIdConnectApplicationAccessLimitErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetOpenIdConnectApplicationAccessLimitErrorCode>(Code, Message, Path);

public sealed record SetOpenIdConnectApplicationAccessLimitPayload(
   OpenIdConnectApplicationAccessPolicy? OpenIdConnectApplicationAccessPolicy,
   IReadOnlyCollection<SetOpenIdConnectApplicationAccessLimitError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetOpenIdConnectApplicationAccessLimitMutation
: MutationBase<OpenIdConnectApplicationAccessPolicy, SetOpenIdConnectApplicationAccessLimitPayload, SetOpenIdConnectApplicationAccessLimitError, SetOpenIdConnectApplicationAccessLimitErrorCode>
{
    protected override SetOpenIdConnectApplicationAccessLimitPayload NewPayload(
        OpenIdConnectApplicationAccessPolicy? data,
        IReadOnlyCollection<SetOpenIdConnectApplicationAccessLimitError>? errors
    ) => new(data, errors);

    protected override SetOpenIdConnectApplicationAccessLimitError NewError(
        SetOpenIdConnectApplicationAccessLimitErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<SetOpenIdConnectApplicationAccessLimitPayload> SetOpenIdConnectApplicationAccessLimitAsync(
        SetOpenIdConnectApplicationAccessLimitInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetOpenIdConnectApplicationAccessLimitErrorCode.UNAUTHENTICATED,
                SetOpenIdConnectApplicationAccessLimitErrorCode.UNAUTHORIZED,
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
            accessPolicy = new OpenIdConnectApplicationAccessPolicy(input.ClientId)
            {
                UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel()
            };
            context.OpenIdConnectApplicationAccessPolicies.Add(accessPolicy);
        }
        else
        {
            accessPolicy.UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel();
            accessPolicy.AccessCountSinceStartTime = null;
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicy, null);
    }
}