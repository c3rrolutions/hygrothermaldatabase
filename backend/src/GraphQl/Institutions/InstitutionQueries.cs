using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.ApiRequests;
using HotChocolate.Authorization;

namespace Database.GraphQl.Institutions;

[ExtendObjectType(nameof(Query))]
public sealed class InstitutionQueries
{
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public Task<QueryCurrentUserOrApplication.CurrentInstitution?> GetCurrentInstitutionAsync(
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return authorization.SwitchUserOrApplicationAsync(
            user => Task.FromResult<QueryCurrentUserOrApplication.CurrentInstitution?>(null),
            application => Task.FromResult<QueryCurrentUserOrApplication.CurrentInstitution?>(application.Owner),
            cancellationToken
        );
    }
}