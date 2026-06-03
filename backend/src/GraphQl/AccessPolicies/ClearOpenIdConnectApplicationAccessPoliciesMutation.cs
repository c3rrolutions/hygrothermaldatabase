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

[SuppressMessage("Naming", "CA1707")]
public enum ClearOpenIdConnectApplicationAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED
}

public sealed record ClearOpenIdConnectApplicationAccessPoliciesError(
    ClearOpenIdConnectApplicationAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ClearOpenIdConnectApplicationAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ClearOpenIdConnectApplicationAccessPoliciesPayload(
   IReadOnlyCollection<OpenIdConnectApplicationAccessPolicy>? OpenIdConnectApplicationAccessPolicies,
   IReadOnlyCollection<ClearOpenIdConnectApplicationAccessPoliciesError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ClearOpenIdConnectApplicationAccessPoliciesMutation
: MutationBase<IReadOnlyCollection<OpenIdConnectApplicationAccessPolicy>, ClearOpenIdConnectApplicationAccessPoliciesPayload, ClearOpenIdConnectApplicationAccessPoliciesError, ClearOpenIdConnectApplicationAccessPoliciesErrorCode>
{
    protected override ClearOpenIdConnectApplicationAccessPoliciesPayload NewPayload(
        IReadOnlyCollection<OpenIdConnectApplicationAccessPolicy>? data,
        IReadOnlyCollection<ClearOpenIdConnectApplicationAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ClearOpenIdConnectApplicationAccessPoliciesError NewError(
        ClearOpenIdConnectApplicationAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<ClearOpenIdConnectApplicationAccessPoliciesPayload> ClearOpenIdConnectApplicationAccessPoliciesAsync(
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ClearOpenIdConnectApplicationAccessPoliciesErrorCode.UNAUTHENTICATED,
                ClearOpenIdConnectApplicationAccessPoliciesErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var accessPolicies = await context.OpenIdConnectApplicationAccessPolicies.ToListAsync(cancellationToken);
        context.OpenIdConnectApplicationAccessPolicies.RemoveRange(accessPolicies);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicies, null);
    }
}