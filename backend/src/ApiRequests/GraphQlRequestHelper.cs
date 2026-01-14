using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Resolvers;

namespace Database.ApiRequests;

public sealed class GraphQlRequestHelper
{
    public static async Task<T> TransformExceptionsAsync<T>(
        Func<Task<T>> action,
        IResolverContext resolverContext,
        Uri databaseLocator
    )
    {
        try
        {
            return await action();
        }
        catch (HttpRequestException e)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetCode("EXTERNAL_GRAPHQL_REQUEST_FAILED")
                    .SetPath(resolverContext.Path)
                    .SetMessage($"Failed with status code '{e.StatusCode}' to request the endpoint '{databaseLocator}'.")
                    .SetException(e)
                    .Build()
            );
        }
        catch (JsonException e)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetCode("JSON_DESERIALIZATION_FAILED")
                    .SetPath(resolverContext.Path) // TODO Add the error path. I would do it as follows as a workaround, however splitting the path at '.' is wrong in general: .SetPath(resolverContext.Path.ToList().Concat(e.Path?.Split('.') ?? []).ToList())
                    .SetMessage($"Failed to deserialize the GraphQL response of the request to the endpoint '{databaseLocator}'. The details given are: Zero-based number of bytes read within the current line before the exception are '{e.BytePositionInLine}', zero-based number of lines read before the exception are '{e.LineNumber}', message that describes the current exception is \"{e.Message}\", path within the JSON where the exception was encountered is '{e.Path}'.")
                    .SetException(e)
                    .Build()
            );
        }
    }
}