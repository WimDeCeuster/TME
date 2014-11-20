using System;
using System.Collections.Generic;
using System.Linq;

namespace TME.CarConfigurator.Repository.Objects.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Recursively flatten a collection.
        /// </summary>
        /// <param name="items">Collection to flatten</param>
        /// <param name="children">Child item accessor</param>
        public static IEnumerable<T> Flatten<T>(this IList<T> items, Func<T, IList<T>> children)
        {
            return items.Concat(items.SelectMany(item => children(item).Flatten(children)));
        }
    }
}
