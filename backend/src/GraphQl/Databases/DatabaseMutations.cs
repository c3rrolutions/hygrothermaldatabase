using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Services;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Mutation))]
public sealed class DatabaseMutations
{
    public async Task<UpdateDatabasePayload> UpdateDatabaseAsync(
        UpdateDatabaseInput input,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        var databasePayload = await DatabaseApi.UpdateDatabase(
            input,
            appSettings,
            apiRequestService,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken).ConfigureAwait(false);
        if (databasePayload is null || databasePayload.Database is null)
        {
           return new UpdateDatabasePayload(
                null,
                [
                    new UpdateDatabaseError(
                    UpdateDatabaseErrorCode.UNKNOWN,
                    "Unknown error.",
                    []
                )
                ]
           );
        }
        return new UpdateDatabasePayload(
            Database.FromDto(databasePayload.Database),
            null
        );
    }
}