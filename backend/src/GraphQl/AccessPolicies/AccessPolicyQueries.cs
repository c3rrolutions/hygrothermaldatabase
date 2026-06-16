using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.GraphQl.Extensions;
using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

[ExtendObjectType(nameof(Query))]
public sealed class AccessPolicyQueries
{
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<DataAccessPolicy?> GetDataAccessPolicyAsync(
        Guid? dataId,
        ApplicationDbContext databaseContext,
        IDataAccessPolicyByDataIdDataLoader byId,
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return null;
        }
        if (dataId is null)
        {
            return await databaseContext.DataAccessPolicies.AsNoTracking()
                .Where(_ => _.DataId == null)
                .SingleAsync(cancellationToken);
        }
        return await byId.LoadAsync(dataId ?? Guid.Empty, cancellationToken);
    }

    [UsePaging]
    [UseFiltering<DataAccessPolicyFilterType>]
    [UseSorting<DataAccessPolicySortType>]
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async ValueTask<HotChocolate.Types.Pagination.Connection<DataAccessPolicy>> GetDataAccessPoliciesAsync(
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        ApplicationDbContext databaseContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return HotChocolate.Types.Pagination.Connection.Empty<DataAccessPolicy>();
        }
        return await databaseContext.DataAccessPolicies
            .AsNoTracking()
            .With(resolverContext.GetQueryContext<DataAccessPolicy>(), Sorting.DefaultEntityOrder)
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    [UsePaging]
    [UseFiltering<UserAccessPolicyFilterType>]
    [UseSorting<UserAccessPolicySortType>]
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async ValueTask<HotChocolate.Types.Pagination.Connection<UserAccessPolicy>> GetUserAccessPoliciesAsync(
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        ApplicationDbContext databaseContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return HotChocolate.Types.Pagination.Connection.Empty<UserAccessPolicy>();
        }
        return await databaseContext.UserAccessPolicies
            .AsNoTracking()
            .With(resolverContext.GetQueryContext<UserAccessPolicy>(), Sorting.DefaultEntityOrder)
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    [UsePaging]
    [UseFiltering<InstitutionAccessPolicyFilterType>]
    [UseSorting<InstitutionAccessPolicySortType>]
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async ValueTask<HotChocolate.Types.Pagination.Connection<InstitutionAccessPolicy>> GetInstitutionAccessPoliciesAsync(
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        ApplicationDbContext databaseContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return HotChocolate.Types.Pagination.Connection.Empty<InstitutionAccessPolicy>();
        }
        return await databaseContext.InstitutionAccessPolicies
            .AsNoTracking()
            .With(resolverContext.GetQueryContext<InstitutionAccessPolicy>(), Sorting.DefaultEntityOrder)
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    [UsePaging]
    [UseFiltering<OpenIdConnectApplicationAccessPolicyFilterType>]
    [UseSorting<OpenIdConnectApplicationAccessPolicySortType>]
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async ValueTask<HotChocolate.Types.Pagination.Connection<OpenIdConnectApplicationAccessPolicy>> GetOpenIdConnectApplicationAccessPoliciesAsync(
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        ApplicationDbContext databaseContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return HotChocolate.Types.Pagination.Connection.Empty<OpenIdConnectApplicationAccessPolicy>();
        }
        return await databaseContext.OpenIdConnectApplicationAccessPolicies
            .AsNoTracking()
            .With(resolverContext.GetQueryContext<OpenIdConnectApplicationAccessPolicy>(), Sorting.DefaultEntityOrder)
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }
}