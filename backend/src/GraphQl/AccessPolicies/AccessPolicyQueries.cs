using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.GraphQl.Extensions;
using GreenDonut.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

[ExtendObjectType(nameof(Query))]
public sealed class AccessPolicyQueries
{
    public Task<UserAccessPolicy?> GetUserAccessPolicyAsync(
        Guid userId,
        IUserAccessPolicyByUserIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId.LoadAsync(userId, cancellationToken);
    }

    public ValueTask<HotChocolate.Types.Pagination.Connection<UserAccessPolicy>> GetUserAccessPoliciesAsync(
        ApplicationDbContext databaseContext,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return databaseContext.UserAccessPolicies.AsNoTracking()
            .With(resolverContext.GetQueryContext<UserAccessPolicy>(), _ => _.AddDescending(_ => _.UserId))
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    public Task<InstitutionAccessPolicy?> GetInstitutionAccessPolicyAsync(
        Guid institutionId,
        IInstitutionAccessPolicyByInstitutionIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId.LoadAsync(institutionId, cancellationToken);
    }

    public ValueTask<HotChocolate.Types.Pagination.Connection<InstitutionAccessPolicy>> GetInstitutionAccessPoliciesAsync(
        ApplicationDbContext databaseContext,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return databaseContext.InstitutionAccessPolicies.AsNoTracking()
            .With(resolverContext.GetQueryContext<InstitutionAccessPolicy>(), _ => _.AddDescending(_ => _.InstitutionId))
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    public Task<OpenIdConnectApplicationAccessPolicy?> GetOpenIdConnectApplicationAccessPolicyAsync(
        string clientId,
        IOpenIdConnectApplicationAccessPolicyByClientIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId.LoadAsync(clientId, cancellationToken);
    }

    public ValueTask<HotChocolate.Types.Pagination.Connection<OpenIdConnectApplicationAccessPolicy>> GetOpenIdConnectApplicationAccessPoliciesAsync(
        ApplicationDbContext databaseContext,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return databaseContext.OpenIdConnectApplicationAccessPolicies.AsNoTracking()
            .With(resolverContext.GetQueryContext<OpenIdConnectApplicationAccessPolicy>(), _ => _.AddDescending(_ => _.ClientId))
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }
}