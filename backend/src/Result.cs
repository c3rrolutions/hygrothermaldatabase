using System.Diagnostics.CodeAnalysis;

namespace Database;

public abstract record Result<TData, TError>
{
    public sealed record Data(TData Value)
    : Result<TData, TError>;

    [SuppressMessage("Naming", "CA1716")]
    public sealed record Error(TError Value)
    : Result<TData, TError>;
}