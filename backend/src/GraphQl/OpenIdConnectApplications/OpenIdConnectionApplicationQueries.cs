using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.ApiRequests;
using HotChocolate.Authorization;

namespace Database.GraphQl.Applications;

[ExtendObjectType(nameof(Query))]
public sealed class ApplicationQueries
{
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public Task<QueryCurrentUserOrApplication.CurrentOpenIdConnectApplication?> GetCurrentOpenIdConnectApplicationAsync(
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return authorization.SwitchUserOrApplicationAsync(
            user => Task.FromResult<QueryCurrentUserOrApplication.CurrentOpenIdConnectApplication?>(null),
            application => Task.FromResult<QueryCurrentUserOrApplication.CurrentOpenIdConnectApplication?>(application),
            cancellationToken
        );
    }
}