using System;
using System.Collections.Generic;
using System.Linq;
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