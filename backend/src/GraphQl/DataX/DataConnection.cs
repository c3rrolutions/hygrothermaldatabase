using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace Database.GraphQl.DataX;

public sealed class DataConnection(
    IReadOnlyList<DataEdge> edges,
    uint totalCount,
    ConnectionPageInfo pageInfo
)
: DataConnectionBase<DataEdge>(
    edges,
    totalCount,
    pageInfo
)
{
}