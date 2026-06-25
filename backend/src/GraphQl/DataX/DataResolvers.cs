using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.GraphQl.AccessPolicies;
using Database.GraphQl.Extensions;
using Database.GraphQl.GetHttpsResources;
using Database.Services;
using GreenDonut;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using System.Linq;
using HotChocolate.Authorization;

namespace Database.GraphQl.DataX;

public sealed class DataResolvers
{
    [UseFiltering<GetHttpsResourceFilterType>]
    [UseSorting<GetHttpsResourceSortType>]
    public Task<GetHttpsResource[]> GetHttpsResourcesAsync(
        [Parent] IData data,
        IResolverContext resolverContext,
        IHttpsResourcesByDataIdDataLoader byId,
        CancellationToken cancellationToken
    )
    {
        return byId
            .With(resolverContext.GetQueryContext<GetHttpsResource>())
            .LoadRequiredAsync(data.Id, cancellationToken);
    }

    public GetHttpsResourceTree GetHttpsResourceTree(
        [Parent] IData data
    )
    {
        return new GetHttpsResourceTree(data);
    }

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<DataAccessPolicy?> GetDataAccessPolicyAsync(
        [Parent] IData data,
        IResolverContext resolverContext,
        IDataAccessPolicyByDataIdDataLoader byId,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return null;
        }
        return await byId
            .With(resolverContext.GetQueryContext<DataAccessPolicy>())
            .LoadRequiredAsync(data.Id, cancellationToken);
    }

    public Task<DatabaseDataLoader.Database?> GetDatabaseAsync(
        [Parent] IData data,
        IDatabaseByIdDataLoader byId,
        AppSettings appSettings
    )
    {
        return byId.LoadAsync(appSettings.DatabaseId);
    }

    public Task<ComponentDataLoader.Component?> GetComponentAsync(
        [Parent] IData data,
        IComponentByIdDataLoader byId
    )
    {
        return byId.LoadAsync(data.ComponentId);
    }

    public Task<InstitutionDataLoader.Institution?> GetInstitutionAsync(
        [Parent] IData data,
        IInstitutionByIdDataLoader byId
    )
    {
        return byId.LoadAsync(data.CreatorId);
    }

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public static async Task<bool> IsNobodyAllowedAsync(
        [Parent] IData data,
        ApplicationDbContext databaseContext,
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return false;
        }
        return await databaseContext.Data(data.Kind)
            .Where(_ => _.Id == data.Id)
            .Where(_ =>
                (
                    _.AccessPolicy != null
                    && _.AccessPolicy.IsNobodyAllowed
                )
                ||
                (
                    !databaseContext.DataAccessPolicies.Any(_ =>
                        _.DataId == null
                        && !_.IsNobodyAllowed
                    )
                )
            )
            .SingleOrDefaultAsync(cancellationToken)
            is not null;
    }

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public static Task<bool> IsAnyoneAllowedAsync(
        [Parent] IData data,
        ApplicationDbContext databaseContext,
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return IsAccessAllowedAsync(data, null, null, null, databaseContext, authorization, resolverContext, cancellationToken);
    }

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public static async Task<bool> IsAccessAllowedAsync(
        [Parent] IData data,
        Guid? userId,
        Guid[]? institutionIds,
        string? openIdConnectClientId,
        ApplicationDbContext databaseContext,
        CommonAuthorization authorization,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return false;
        }
        IQueryable<IData> policedQuery = data.Kind switch
        {
            DataKind.CALORIMETRIC_DATA => AccessPolicyService.PoliceData<CalorimetricData>(databaseContext.CalorimetricData, userId, institutionIds, openIdConnectClientId, databaseContext),
            DataKind.GEOMETRIC_DATA => AccessPolicyService.PoliceData<GeometricData>(databaseContext.GeometricData, userId, institutionIds, openIdConnectClientId, databaseContext),
            DataKind.HYGROTHERMAL_DATA => AccessPolicyService.PoliceData<HygrothermalData>(databaseContext.HygrothermalData, userId, institutionIds, openIdConnectClientId, databaseContext),
            DataKind.LIFE_CYCLE_DATA => AccessPolicyService.PoliceData<LifeCycleData>(databaseContext.LifeCycleData, userId, institutionIds, openIdConnectClientId, databaseContext),
            DataKind.OPTICAL_DATA => AccessPolicyService.PoliceData<OpticalData>(databaseContext.OpticalData, userId, institutionIds, openIdConnectClientId, databaseContext),
            DataKind.PHOTOVOLTAIC_DATA => AccessPolicyService.PoliceData<PhotovoltaicData>(databaseContext.PhotovoltaicData, userId, institutionIds, openIdConnectClientId, databaseContext),
            _ => throw new ArgumentOutOfRangeException(nameof(data), $"Unsupported data kind {data.Kind}"),
        };
        return await policedQuery
            .Where(_ => _.Id == data.Id)
            .SingleOrDefaultAsync(cancellationToken)
            is not null;
    }
}