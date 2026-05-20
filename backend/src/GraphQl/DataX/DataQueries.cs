using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Enumerations;
using Database.GraphQl.Scalars;
using Database.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.DataX;

[ExtendObjectType(nameof(Query))]
public sealed class DataQueries
{
    public async Task<IData?> GetDataAsync(
        Guid id,
        DataKind dataKind,
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext dbContext,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var data = await dbContext.GetDataAsync(id, dataKind, cancellationToken);
        if (data is null)
        {
            return null;
        }
        return await accessRightsService.ApplyAccessRightsOnData(data, cancellationToken);
    }
}