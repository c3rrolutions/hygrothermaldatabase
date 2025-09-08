using System.Collections.Generic;
using System.Linq;

namespace Database.Extensions;

public static class EnumerableExtensions
{
    // An alternative would be `Index` from https://github.com/morelinq/MoreLINQ
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}