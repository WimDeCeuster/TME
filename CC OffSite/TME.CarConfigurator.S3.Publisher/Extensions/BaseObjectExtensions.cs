using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.S3.Publisher.Extensions
{
    public static class BaseObjectExtensions
    {
        public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> baseObjects)
            where T : BaseObject
        {
            return baseObjects.OrderBy(baseObject => baseObject.SortIndex)
                              .ThenBy(baseObject => baseObject.Name);
        }
    }
}
