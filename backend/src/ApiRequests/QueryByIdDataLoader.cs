using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;

namespace Database.ApiRequests;

public interface IIdNode
{
    Guid Uuid { get; }
}

public sealed record Edge<TNode>(
    TNode Node
);

public sealed record Connection<TNode>(
    IReadOnlyCollection<Edge<TNode>>? Edges
);

public interface IConnectionData<TNode>
{
    Connection<TNode>? Connection { get; }
}


public static class QueryByIdDataLoader
{
    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        appSettings.MetabaseGraphQlEndpoint;

    public static async Task<Dictionary<Guid, TNode>> GetByIdAsync<TData, TNode>(
        IReadOnlyList<Guid> ids,
        string[] queryFileNames,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    where TData : class, IConnectionData<TNode>
    where TNode : IIdNode
    {
        return (await apiRequestService.QueryGraphQl<TData>(
            GetGraphQlEndpoint(appSettings),
            new GraphQLRequest(
                await apiRequestService.ConstructGraphQlQuery(queryFileNames),
                new
                {
                    ids
                },
                null
            ),
            cancellationToken
        ))
        .Data
        ?.Connection
        ?.Edges
        ?.ToDictionary(
            _ => _.Node.Uuid,
            _ => _.Node
        ) ?? Enumerable.Empty<TNode>().ToDictionary(_ => _.Uuid);
    }
}