using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using Database.GraphQl.AccessPolicies;
using Database.GraphQl.Extensions;
using HotChocolate.Data;
using HotChocolate.Authorization;
using HotChocolate.Data.Filters;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicy;

[SuppressMessage("Naming", "CA1707")]
public enum ResetDataAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED
}

public sealed record ResetDataAccessPoliciesError(
    ResetDataAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ResetDataAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ResetDataAccessPoliciesPayload(
   IReadOnlyCollection<DataAccessPolicy>? DataAccessPolicies,
   IReadOnlyCollection<ResetDataAccessPoliciesError>? Errors
) : Payload;

public sealed class ResetDataAccessPoliciesFilterType
    : DataAccessPolicyFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<DataAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(ResetDataAccessPoliciesFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class ResetDataAccessPoliciesMutation
: MutationBase<IReadOnlyCollection<DataAccessPolicy>, ResetDataAccessPoliciesPayload, ResetDataAccessPoliciesError, ResetDataAccessPoliciesErrorCode>
{
    protected override ResetDataAccessPoliciesPayload NewPayload(
        IReadOnlyCollection<DataAccessPolicy>? data,
        IReadOnlyCollection<ResetDataAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ResetDataAccessPoliciesError NewError(
        ResetDataAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [UseFiltering<ResetDataAccessPoliciesFilterType>]
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<ResetDataAccessPoliciesPayload> ResetDataAccessPoliciesAsync(
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ResetDataAccessPoliciesErrorCode.UNAUTHENTICATED,
                ResetDataAccessPoliciesErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        var policies =
            await context.DataAccessPolicies
            .Include(_ => _.UserAccessPolicies)
            .Include(_ => _.InstitutionAccessPolicies)
            .Include(_ => _.OpenIdConnectApplicationAccessPolicies)
            .With(resolverContext.GetQueryContext<DataAccessPolicy>(), Sorting.DefaultEntityOrder)
            .ToListAsync(cancellationToken);
        foreach (var policy in policies)
        {
            policy.Reset();
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(policies, null);
    }
}
