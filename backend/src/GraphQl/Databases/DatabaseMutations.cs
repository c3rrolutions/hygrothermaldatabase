using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Services.Interfaces;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Mutation))]
public sealed class DatabaseMutations
{
    public async Task<UpdateDatabasePayload> UpdateDatabaseAsync(
        UpdateDatabaseInput input,
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        var database = await DatabaseApi.UpdateDatabase(
            input,
            appSettings,
            apiRequestService,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken).ConfigureAwait(false);
        return database is not null ? new UpdateDatabasePayload(
            Database.FromDto(database),
            null) : new UpdateDatabasePayload(
                   null,
                   new[]
                   {
                       new UpdateDatabaseError(
                           UpdateDatabaseErrorCode.UNKNOWN,
                           "Unknown error.",
                           Array.Empty<string>()
                       )
                   }
               );
    }
}