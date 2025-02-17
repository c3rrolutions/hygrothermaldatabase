using Database.Data;

namespace Database.GraphQl.DataX;

public sealed class DataEdge(
    string cursor,
    IData node
    )
        : DataEdgeBase<IData>(
        cursor,
        node
        )
{
}