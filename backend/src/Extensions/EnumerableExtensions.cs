using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Database.Extensions;

public static class EnumerableExtensions
{
    [Pure]
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : class
    {
        return enumerable.Where(item => item is not null).Select(item => item!);
    }

    [Pure]
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : struct
    {
        return enumerable.Where(item => item.HasValue).Select(item => item!.Value);
    }
}