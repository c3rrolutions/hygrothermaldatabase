using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data.AccessPolicies;
using Database.GraphQl.AccessPolicies;
using Database.GraphQl.Extensions;
using GreenDonut;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Database.GraphQl;

public sealed class DataAccessPolicyType
    : ObjectType<DataAccessPolicy>
{
    protected override void Configure(
        IObjectTypeDescriptor<DataAccessPolicy> descriptor
    )
    {
        base.Configure(descriptor);
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
    }

    private sealed class Resolvers
    {
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
                return Array.Empty<UserAccessPolicy>();
            }
            return await byId
                .With(resolverContext.GetQueryContext<UserAccessPolicy>())
                .LoadRequiredAsync(dataAccessPolicy.Id, cancellationToken);
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
                return Array.Empty<InstitutionAccessPolicy>();
            }
            return await byId
                .With(resolverContext.GetQueryContext<InstitutionAccessPolicy>())
                .LoadRequiredAsync(dataAccessPolicy.Id, cancellationToken);
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
                return Array.Empty<OpenIdConnectApplicationAccessPolicy>();
            }
            return await byId
                .With(resolverContext.GetQueryContext<OpenIdConnectApplicationAccessPolicy>())
                .LoadRequiredAsync(dataAccessPolicy.Id, cancellationToken);
        }
    }
}