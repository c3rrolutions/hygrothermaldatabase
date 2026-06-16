using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Enumerations;
using Database.GraphQl.Scalars;
using Database.Services;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

[ExtendObjectType(nameof(Query))]
public sealed class DataQueries
{
    public Task<IData?> GetDataAsync(
        Guid id,
        DataKind dataKind,
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext databaseContext,
        AccessPolicyService accessPolicyService,
        CancellationToken cancellationToken
    )
    {
        return accessPolicyService.Apply<IData, IData?>(
            databaseContext.Data(dataKind).AsNoTracking()
                .Where(_ => _.Id == id),
            async policedData =>
            {
                var node = await policedData.SingleOrDefaultAsync(cancellationToken);
                return (node is null ? [] : [node], node);
            },
            databaseContext,
            cancellationToken
        );
    }
}