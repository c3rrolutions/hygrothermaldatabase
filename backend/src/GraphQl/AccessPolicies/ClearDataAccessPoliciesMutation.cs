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

namespace Database.GraphQl.AccessPolicy;

[SuppressMessage("Naming", "CA1707")]
public enum ClearDataAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record ClearDataAccessPoliciesError(
    ClearDataAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ClearDataAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ClearDataAccessPoliciesPayload(
   IReadOnlyCollection<DataAccessPolicy>? DataAccessPolicy,
   IReadOnlyCollection<ClearDataAccessPoliciesError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ClearDataAccessPoliciesMutation
: MutationBase<IReadOnlyCollection<DataAccessPolicy>, ClearDataAccessPoliciesPayload, ClearDataAccessPoliciesError, ClearDataAccessPoliciesErrorCode>
{
    protected override ClearDataAccessPoliciesPayload NewPayload(
        IReadOnlyCollection<DataAccessPolicy>? data,
        IReadOnlyCollection<ClearDataAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ClearDataAccessPoliciesError NewError(
        ClearDataAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<ClearDataAccessPoliciesPayload> ClearDataAccessPoliciesAsync(
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ClearDataAccessPoliciesErrorCode.UNAUTHENTICATED,
                ClearDataAccessPoliciesErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var policies = await context.DataAccessPolicies.ToListAsync(cancellationToken);
        context.DataAccessPolicies.RemoveRange(policies);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(policies, null);
    }
}