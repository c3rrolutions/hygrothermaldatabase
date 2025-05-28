using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Services.Interfaces;
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
        IDataService dataService,
        IAccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        var data = await dataService.GetDataAsync(id, dbContext, cancellationToken).ConfigureAwait(false);

        if (data is null)
        {
            return data;
        }

        return await accessRightsService.ApplyAccessRightsOnData(data, cancellationToken).ConfigureAwait(false);
    }
}