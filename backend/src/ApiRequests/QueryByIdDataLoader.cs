using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;

namespace Database.ApiRequests;

public static class QueryByIdDataLoader
{
    public interface IIdNode<TId>
    {
        TId Id { get; }
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

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        appSettings.MetabaseGraphQlEndpoint;

    public static async Task<Dictionary<TId, TNode>> GetByIdAsync<TId, TData, TNode>(
        IReadOnlyList<TId> ids,
        string[] queryFileNames,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    where TId : notnull
    where TData : class, IConnectionData<TNode>
    where TNode : IIdNode<TId>
    {
        return (await apiRequestService.QueryGraphQl<TData>(
            GetGraphQlEndpoint(appSettings),
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(queryFileNames),
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
            _ => _.Node.Id,
            _ => _.Node
        ) ?? Enumerable.Empty<TNode>().ToDictionary(_ => _.Id);
    }
}