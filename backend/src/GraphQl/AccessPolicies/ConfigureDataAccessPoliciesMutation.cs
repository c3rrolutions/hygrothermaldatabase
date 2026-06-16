using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.Extensions;
using Database.GraphQl.AccessPolicies;
using Database.GraphQl.Extensions;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicy;

public sealed record ConfigureDataAccessPoliciesInput
(
    LogicalCombinator Combinator
);

[SuppressMessage("Naming", "CA1707")]
public enum ConfigureDataAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED
}

public sealed record ConfigureDataAccessPoliciesError(
    ConfigureDataAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ConfigureDataAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ConfigureDataAccessPoliciesPayload(
   IReadOnlyCollection<DataAccessPolicy>? DataAccessPolicies,
   IReadOnlyCollection<ConfigureDataAccessPoliciesError>? Errors
) : Payload;

public sealed class ConfigureDataAccessPoliciesFilterType
    : DataAccessPolicyFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<DataAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(ConfigureDataAccessPoliciesFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class ConfigureDataAccessPoliciesMutation
: MutationBase<IReadOnlyCollection<DataAccessPolicy>, ConfigureDataAccessPoliciesPayload, ConfigureDataAccessPoliciesError, ConfigureDataAccessPoliciesErrorCode>
{
    protected override ConfigureDataAccessPoliciesPayload NewPayload(
        IReadOnlyCollection<DataAccessPolicy>? data,
        IReadOnlyCollection<ConfigureDataAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ConfigureDataAccessPoliciesError NewError(
        ConfigureDataAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [UseFiltering<ConfigureDataAccessPoliciesFilterType>]
    public async Task<ConfigureDataAccessPoliciesPayload> ConfigureDataAccessPoliciesAsync(
        ConfigureDataAccessPoliciesInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ConfigureDataAccessPoliciesErrorCode.UNAUTHENTICATED,
                ConfigureDataAccessPoliciesErrorCode.UNAUTHORIZED,
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
            .With(resolverContext.GetQueryContext<DataAccessPolicy>(), Sorting.DefaultEntityOrder)
            .ToListAsync(cancellationToken);
        foreach (var policy in policies)
        {
            policy.Configure(input.Combinator);
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(policies, null);
    }
}