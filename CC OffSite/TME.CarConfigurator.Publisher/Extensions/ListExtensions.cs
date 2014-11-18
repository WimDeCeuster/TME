using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> items)
        {
            return (IReadOnlyList<T>)items.ToList();
        }
    }
}
