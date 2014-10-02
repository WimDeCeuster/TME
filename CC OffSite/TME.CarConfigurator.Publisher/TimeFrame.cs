using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher
{
    public class TimeFrame
    {
        public readonly DateTime From;
        public readonly DateTime Until;

        public readonly IReadOnlyList<Car> Cars;
        public readonly Guid ID;

        public TimeFrame(DateTime from, DateTime until, IReadOnlyList<Car> cars)
        {
            if (cars == null) throw new ArgumentNullException("cars");

            From = from;
            Until = until;
            Cars = cars;

            ID = Guid.NewGuid();
        }
    }
}
