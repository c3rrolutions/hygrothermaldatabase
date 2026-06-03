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
public enum ClearInstitutionAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED
}

public sealed record ClearInstitutionAccessPoliciesError(
    ClearInstitutionAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ClearInstitutionAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ClearInstitutionAccessPoliciesPayload(
   IReadOnlyCollection<InstitutionAccessPolicy>? InstitutionAccessPolicies,
   IReadOnlyCollection<ClearInstitutionAccessPoliciesError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ClearInstitutionAccessPoliciesMutation
: MutationBase<IReadOnlyCollection<InstitutionAccessPolicy>, ClearInstitutionAccessPoliciesPayload, ClearInstitutionAccessPoliciesError, ClearInstitutionAccessPoliciesErrorCode>
{
    protected override ClearInstitutionAccessPoliciesPayload NewPayload(
        IReadOnlyCollection<InstitutionAccessPolicy>? data,
        IReadOnlyCollection<ClearInstitutionAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ClearInstitutionAccessPoliciesError NewError(
        ClearInstitutionAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<ClearInstitutionAccessPoliciesPayload> ClearInstitutionAccessPoliciesAsync(
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ClearInstitutionAccessPoliciesErrorCode.UNAUTHENTICATED,
                ClearInstitutionAccessPoliciesErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var accessPolicies = await context.InstitutionAccessPolicies.ToListAsync(cancellationToken);
        context.InstitutionAccessPolicies.RemoveRange(accessPolicies);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicies, null);
    }
}