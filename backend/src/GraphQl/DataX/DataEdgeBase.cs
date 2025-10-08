namespace Database.GraphQl.DataX;

public abstract record DataEdgeBase<TData>(
    string Cursor,
    TData Node
);