using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class TimeFrameBuilder
    {
        private DateTime From;
        private DateTime Until;
        private IEnumerable<Car> Cars;

        public TimeFrameBuilder WithDateRange(DateTime from, DateTime until)
        {
            From = from;
            Until = until;

            return this;
        }

        public TimeFrameBuilder WithCars(IEnumerable<Car> cars)
        {
            Cars = cars;
            return this;
        }

        public TimeFrame Build()
        {
            return new TimeFrame(From, Until, Cars.ToList());
        }
    }
}