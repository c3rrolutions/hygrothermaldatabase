using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.Services;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace Database.GraphQl.Users;

[ExtendObjectType(nameof(Query))]
public sealed class UserQueries
{
    public async Task<User?> GetCurrentUserAsync(
        ClaimsPrincipal claimsPrincipal,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if (!claimsPrincipal.HasClaim(ClaimTypes.NameIdentifier)) return null;
        return
            await context.Users.AsNoTracking()
                .SingleOrDefaultAsync(
                    u => claimsPrincipal.GetClaims(ClaimTypes.NameIdentifier).Contains(u.Subject),
                    cancellationToken
                ).ConfigureAwait(false);
    }

    public async Task<UserInfo?> GetCurrentUserInfoAsync(
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return UserInfo.FromDto(await UserApi.RequestUserInfo(
                appSettings,
                apiRequestService,
                httpClientFactory,
                httpContextAccessor,
                cancellationToken
            ));
        }
        catch (HttpRequestException e)
        {
            resolverContext.ReportError(
                ErrorBuilder.New()
                    .SetCode("METABASE_REQUEST_FAILED")
                    .SetPath(resolverContext.Path)
                    .SetMessage($"Failed with status code {e.StatusCode} to request {appSettings.MetabaseHost}.")
                    .SetException(e)
                    .Build()
            );
            return null;
        }
        catch (JsonException e)
        {
            resolverContext.ReportError(
                ErrorBuilder.New()
            .SetCode("JSON_DESERIALIZATION_FAILED")
                    .SetPath(resolverContext.Path.ToList().Concat(e.Path?.Split('.') ?? Array.Empty<string>())
                        .ToList()) // TODO Splitting the path at '.' is wrong in general.
                    .SetMessage(
                        $"Failed to deserialize GraphQL response of request to {appSettings.MetabaseHost}. The details given are: Zero-based number of bytes read within the current line before the exception are {e.BytePositionInLine}, zero-based number of lines read before the exception are {e.LineNumber}, message that describes the current exception is '{e.Message}', path within the JSON where the exception was encountered is {e.Path}.")
                    .SetException(e)
                    .Build()
            );
            return null;
        }
    }
}