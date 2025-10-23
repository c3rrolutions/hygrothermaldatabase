using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Database.Extensions;

public static class EnumerableExtensions
{
    // An alternative would be `Index` from https://github.com/morelinq/MoreLINQ
    [Pure]
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((item, index) => (item, index));
    }

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