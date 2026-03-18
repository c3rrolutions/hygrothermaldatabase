using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.ApiRequests;

namespace Database.GraphQl.Institutions;

[ExtendObjectType(nameof(Query))]
public sealed class InstitutionQueries
{
    public Task<QueryCurrentUserOrInstitution.CurrentInstitution?> GetCurrentInstitutionAsync(
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return authorization.SwitchUserOrInstitutionAsync(
            user => Task.FromResult<QueryCurrentUserOrInstitution.CurrentInstitution?>(null),
            institution => Task.FromResult<QueryCurrentUserOrInstitution.CurrentInstitution?>(institution),
            cancellationToken
        );
    }
}