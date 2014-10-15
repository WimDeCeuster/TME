﻿using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarBuilder
    {
        private readonly Car _car;

        public CarBuilder()
        {
            _car = new Car();
        }

        public CarBuilder WithId(Guid id)
        {
            _car.ID = id;

            return this;
        }

        public CarBuilder WithLabels(params Repository.Objects.Core.Label[] labels)
        {
            _car.Labels = labels.ToList();
         
            return this;
        }

        public Car Build()
        {
            return _car;
        }
    }
}
