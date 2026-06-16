using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.GraphQl.DataX;
using Database.GraphQl.Extensions;
using GreenDonut;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed class DataAccessPolicyType
    : ObjectType<DataAccessPolicy>
{
    protected override void Configure(
        IObjectTypeDescriptor<DataAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(_ => _.GetData(default!))
            .Ignore();
        descriptor
            .Field(_ => _.GetDataId(default!))
            .Ignore();
        descriptor
            .Field(_ => _.CalorimetricDataId)
            .Ignore();
        descriptor
            .Field(_ => _.CalorimetricData)
            .Ignore();
        descriptor
            .Field(_ => _.GeometricDataId)
            .Ignore();
        descriptor
            .Field(_ => _.GeometricData)
            .Ignore();
        descriptor
            .Field(_ => _.HygrothermalDataId)
            .Ignore();
        descriptor
            .Field(_ => _.HygrothermalData)
            .Ignore();
        descriptor
            .Field(_ => _.LifeCycleDataId)
            .Ignore();
        descriptor
            .Field(_ => _.LifeCycleData)
            .Ignore();
        descriptor
            .Field(_ => _.OpticalDataId)
            .Ignore();
        descriptor
            .Field(_ => _.OpticalData)
            .Ignore();
        descriptor
            .Field(_ => _.PhotovoltaicDataId)
            .Ignore();
        descriptor
            .Field(_ => _.PhotovoltaicData)
            .Ignore();
        descriptor
            .Field(_ => _.DataId)
            .Ignore();
        descriptor
            .Field(_ => _.DataKind)
            .Ignore();
        descriptor
            .Field(_ => _.Data)
            .Type<InterfaceType<IData>>()
            .Cost(0)
            .ResolveWith<Resolvers>(t =>
                Resolvers.GetData(default!, default!, default!));
        descriptor
            .Field(_ => _.UserAccessPolicies)
            .Cost(1)
            .ResolveWith<Resolvers>(_ => Resolvers.GetUserAccessPoliciesAsync(default!, default!, default!, default!, default!));
        descriptor
            .Field(_ => _.InstitutionAccessPolicies)
            .Cost(1)
            .ResolveWith<Resolvers>(_ => Resolvers.GetInstitutionAccessPoliciesAsync(default!, default!, default!, default!, default!));
        descriptor
            .Field(_ => _.OpenIdConnectApplicationAccessPolicies)
            .Cost(1)
            .ResolveWith<Resolvers>(_ => Resolvers.GetOpenIdConnectApplicationAccessPoliciesAsync(default!, default!, default!, default!, default!));
        descriptor
            .Field(_ => _.IsAnyoneAllowed)
            .Cost(0)
            .ResolveWith<Resolvers>(t =>
                Resolvers.IsAnyoneAllowedAsync(default!, default!, default!, default!));
        descriptor
            .Field(_ => _.IsAccessAllowed(default!, default!, default!))
            .Cost(0)
            .ResolveWith<Resolvers>(t =>
                Resolvers.IsAccessAllowedAsync(default!, default!, default!, default!, default!, default!, default!));
    }

    private sealed class Resolvers
    {
        public static async Task<IData?> GetData(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IDataByIdAndKindDataLoader dataByIdAndKindDataLoader,
            CancellationToken cancellationToken
        )
        {
            if (dataAccessPolicy.DataId is null || dataAccessPolicy.DataKind is null)
            {
                return null;
            }
            return await dataByIdAndKindDataLoader.LoadRequiredAsync(
                (dataAccessPolicy.DataId ?? Guid.Empty, dataAccessPolicy.DataKind ?? default),
                cancellationToken
            );
        }

        public static Task<bool> IsAnyoneAllowedAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IDataAccessPolicyByDataIdDataLoader policyByDataIdDataLoader,
            ApplicationDbContext databaseContext,
            CancellationToken cancellationToken
        )
        {
            return IsAccessAllowedAsync(dataAccessPolicy, null, null, null, policyByDataIdDataLoader, databaseContext, cancellationToken);
        }

        public static async Task<bool> IsAccessAllowedAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            Guid? userId,
            Guid[]? institutionIds,
            string? openIdConnectClientId,
            IDataAccessPolicyByDataIdDataLoader policyByDataIdDataLoader,
            ApplicationDbContext databaseContext,
            CancellationToken cancellationToken
        )
        {
            if (dataAccessPolicy.DataId is null)
            {
                return await databaseContext.DataAccessPolicies.AsQueryable()
                    .Where(_ => _.IsAccessAllowed(userId, institutionIds, openIdConnectClientId))
                    .SingleOrDefaultAsync(
                        _ => _.DataId == null,
                        cancellationToken
                    )
                    is not null;
            }
            return await policyByDataIdDataLoader
                .Where(_ => _.IsAccessAllowed(userId, institutionIds, openIdConnectClientId))
                .LoadAsync(
                    dataAccessPolicy.DataId ?? Guid.Empty,
                    cancellationToken
                )
                is not null;
        }

        [UseFiltering<UserAccessPolicyFilterType>]
        [UseSorting<UserAccessPolicySortType>]
        public static async Task<UserAccessPolicy[]> GetUserAccessPoliciesAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IResolverContext resolverContext,
            IUserAccessPoliciesByDataAccessPolicyIdDataLoader byId,
            CommonAuthorization authorization,
            CancellationToken cancellationToken
        )
        {
            if (!await authorization.IsDatabaseOperator(cancellationToken))
            {
                authorization.ReportUnauthorizedError(resolverContext);
                return Array.Empty<UserAccessPolicy>();
            }
            return await byId
                .With(resolverContext.GetQueryContext<UserAccessPolicy>())
                .LoadAsync(dataAccessPolicy.Id, cancellationToken)
                ?? [];
        }

        [UseFiltering<InstitutionAccessPolicyFilterType>]
        [UseSorting<InstitutionAccessPolicySortType>]
        public static async Task<InstitutionAccessPolicy[]> GetInstitutionAccessPoliciesAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IResolverContext resolverContext,
            IInstitutionAccessPoliciesByDataAccessPolicyIdDataLoader byId,
            CommonAuthorization authorization,
            CancellationToken cancellationToken
        )
        {
            if (!await authorization.IsDatabaseOperator(cancellationToken))
            {
                authorization.ReportUnauthorizedError(resolverContext);
                return Array.Empty<InstitutionAccessPolicy>();
            }
            return await byId
                .With(resolverContext.GetQueryContext<InstitutionAccessPolicy>())
                .LoadAsync(dataAccessPolicy.Id, cancellationToken)
                ?? [];
        }

        [UseFiltering<OpenIdConnectApplicationAccessPolicyFilterType>]
        [UseSorting<OpenIdConnectApplicationAccessPolicySortType>]
        public static async Task<OpenIdConnectApplicationAccessPolicy[]> GetOpenIdConnectApplicationAccessPoliciesAsync(
            [Parent] DataAccessPolicy dataAccessPolicy,
            IResolverContext resolverContext,
            IOpenIdConnectApplicationAccessPoliciesByDataAccessPolicyIdDataLoader byId,
            CommonAuthorization authorization,
            CancellationToken cancellationToken
        )
        {
            if (!await authorization.IsDatabaseOperator(cancellationToken))
            {
                authorization.ReportUnauthorizedError(resolverContext);
                return Array.Empty<OpenIdConnectApplicationAccessPolicy>();
            }
            return await byId
                .With(resolverContext.GetQueryContext<OpenIdConnectApplicationAccessPolicy>())
                .LoadAsync(dataAccessPolicy.Id, cancellationToken)
                ?? [];
        }
    }
}