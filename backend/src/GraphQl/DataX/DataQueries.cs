using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.DataX;

[ExtendObjectType(nameof(Query))]
public sealed class DataQueries
{
    public async Task<IData?> GetDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext dbContext,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var data = await dbContext.GetDataAsync(id, cancellationToken);
        if (data is null)
        {
            return null;
        }
        return await accessRightsService.ApplyAccessRightsOnData(data, cancellationToken);
    }
}