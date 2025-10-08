using Database.Data;

namespace Database.GraphQl.DataX;

public sealed record DataEdge(
    string Cursor,
    IData Node
    )
        : DataEdgeBase<IData>(
        Cursor,
        Node
        )
{
}