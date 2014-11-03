using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Repository.Objects.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Recursively flatten a collection.
        /// </summary>
        /// <param name="items">Collection to flatten</param>
        /// <param name="children">Child item accessor</param>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> children)
        {
            return items.Concat(items.SelectMany(item => children(item).Flatten(children)));
        }
    }
}
