using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarPackBuilder
    {
        private CarPack _carPack;

        public CarPackBuilder()
        {
            _carPack = new CarPack();
        }

        public CarPackBuilder WithId(Guid ID)
        {
            _carPack.ID = ID;
            return this;
        }

        public CarPackBuilder WithPrice(Price price)
        {
            _carPack.Price = price;
            return this;
        }

        public CarPack Build()
        {
            return _carPack;
        }
    }
}