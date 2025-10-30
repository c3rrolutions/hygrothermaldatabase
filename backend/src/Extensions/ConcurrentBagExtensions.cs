using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Database.Extensions;

public static class ConcurrentBagExtensions
{
    public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T>? toAdd)
    {
        if (toAdd is not null)
        {
            foreach (var item in toAdd)
            {
                bag.Add(item);
            }
        }
    }
}