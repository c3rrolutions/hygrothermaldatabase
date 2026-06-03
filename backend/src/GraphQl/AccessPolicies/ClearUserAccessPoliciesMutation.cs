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
public enum ClearUserAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED
}

public sealed record ClearUserAccessPoliciesError(
    ClearUserAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ClearUserAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ClearUserAccessPoliciesPayload(
   IReadOnlyCollection<UserAccessPolicy>? UserAccessPolicies,
   IReadOnlyCollection<ClearUserAccessPoliciesError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ClearUserAccessPoliciesMutation
: MutationBase<IReadOnlyCollection<UserAccessPolicy>, ClearUserAccessPoliciesPayload, ClearUserAccessPoliciesError, ClearUserAccessPoliciesErrorCode>
{
    protected override ClearUserAccessPoliciesPayload NewPayload(
        IReadOnlyCollection<UserAccessPolicy>? data,
        IReadOnlyCollection<ClearUserAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ClearUserAccessPoliciesError NewError(
        ClearUserAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<ClearUserAccessPoliciesPayload> ClearUserAccessPoliciesAsync(
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ClearUserAccessPoliciesErrorCode.UNAUTHENTICATED,
                ClearUserAccessPoliciesErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var accessPolicies = await context.UserAccessPolicies.ToListAsync(cancellationToken);
        context.UserAccessPolicies.RemoveRange(accessPolicies);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicies, null);
    }
}