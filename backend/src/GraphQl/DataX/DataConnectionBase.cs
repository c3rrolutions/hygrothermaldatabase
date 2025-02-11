using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;

namespace Database.GraphQl.DataX;

public abstract class DataConnectionBase<TDataEdge>(
    IReadOnlyList<TDataEdge> edges,
    ConnectionPageInfo pageInfo
    )
{
    public IReadOnlyList<TDataEdge> Edges { get; } = edges;

    public ConnectionPageInfo PageInfo { get; } = pageInfo;
}