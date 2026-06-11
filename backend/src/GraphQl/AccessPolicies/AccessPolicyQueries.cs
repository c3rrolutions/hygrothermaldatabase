using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using GreenDonut.Data;
using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

[ExtendObjectType(nameof(Query))]
public sealed class AccessPolicyQueries
{
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<DataAccessPolicy?> GetGlobalDataAccessPolicyAsync(
        CommonAuthorization authorization,
        ApplicationDbContext databaseContext,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            authorization.ReportUnauthorizedError(resolverContext);
            return null;
        }
        return await databaseContext.DataAccessPolicies.AsNoTracking()
            .Where(_ => _.DataId == null)
            .SingleAsync(cancellationToken);
    }
}