using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicy;

[SuppressMessage("Naming", "CA1707")]
public enum ClearDataAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record ClearDataAccessPolicyError(
    ClearDataAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ClearDataAccessPolicyErrorCode>(Code, Message, Path);

public sealed record ClearDataAccessPolicyPayload(
   IReadOnlyCollection<DataAccessPolicy>? DataAccessPolicy,
   IReadOnlyCollection<ClearDataAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ClearDataAccessPolicyMutation
: MutationBase<IReadOnlyCollection<DataAccessPolicy>, ClearDataAccessPolicyPayload, ClearDataAccessPolicyError, ClearDataAccessPolicyErrorCode>
{
    protected override ClearDataAccessPolicyPayload NewPayload(
        IReadOnlyCollection<DataAccessPolicy>? data,
        IReadOnlyCollection<ClearDataAccessPolicyError>? errors
    ) => new(data, errors);

    protected override ClearDataAccessPolicyError NewError(
        ClearDataAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<ClearDataAccessPolicyPayload> ClearDataAccessPolicyAsync(
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ClearDataAccessPolicyErrorCode.UNAUTHENTICATED,
                ClearDataAccessPolicyErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var accessPolicies = new List<DataAccessPolicy>();
        await foreach (var data in context.GetAllDataAsync())
        {
            if (data.AccessPolicy is not null)
            {
                accessPolicies.Add(data.AccessPolicy);
                data.AccessPolicy = null;
            }
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicies, null);
    }
}