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
        DataService dataService,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var data = await dataService.GetDataAsync(id, dbContext, cancellationToken);
        if (data is null)
        {
            return null;
        }
        return await accessRightsService.ApplyAccessRightsOnData(data, cancellationToken);
    }
}