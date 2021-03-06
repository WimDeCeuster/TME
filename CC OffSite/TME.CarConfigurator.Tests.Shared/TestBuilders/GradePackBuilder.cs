using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class GradePackBuilder
    {
        private Guid _id;
        private readonly List<CarInfo> _notAvailableOn = new List<CarInfo>();
        private readonly List<CarInfo> _optionalOn = new List<CarInfo>();
        private readonly List<CarInfo> _standardOn = new List<CarInfo>();

        public GradePackBuilder WithID(Guid id)
        {
            _id = id;

            return this;
        }

        public GradePack Build()
        {
            return new GradePack { ID = _id, NotAvailableOn = _notAvailableOn, OptionalOn = _optionalOn, StandardOn = _standardOn };
        }

        public GradePackBuilder AddNotAvailableOn(CarInfo carInfo)
        {
            _notAvailableOn.Add(carInfo);

            return this;
        }

        public GradePackBuilder AddOptionalOn(CarInfo carInfo)
        {
            _optionalOn.Add(carInfo);

            return this;
        }

        public GradePackBuilder AddStandardOn(CarInfo carInfo)
        {
            _standardOn.Add(carInfo);

            return this;
        }
    }
}