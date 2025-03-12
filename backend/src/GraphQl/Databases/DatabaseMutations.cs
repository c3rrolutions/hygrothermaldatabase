using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Mutation))]
public sealed class DatabaseMutations
{
    public async Task<UpdateDatabasePayload> UpdateDatabaseAsync(
        UpdateDatabaseInput input,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        var database = await DatabaseApi.UpdateDatabase(
            new ApiRequest.Dto.DatabaseRequestDto(
                input.DatabaseId,
                input.Description,
                input.Name,
                input.Locator),
            appSettings,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken).ConfigureAwait(false);
        return database is not null ? new UpdateDatabasePayload(
            Data.Database.FromDto(database),
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