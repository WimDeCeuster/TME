using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class CarsExtensions
    {
        public static IEnumerable<Car> Filter(this IEnumerable<Car> cars, bool isPreview)
        {
            return cars.Where(c => (isPreview && c.Preview) || (!isPreview && c.Approved));
        } 
    }
}