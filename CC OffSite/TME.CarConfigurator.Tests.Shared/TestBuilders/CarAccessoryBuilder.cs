using System;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarAccessoryBuilder
    {
        private readonly CarAccessory _carAccessory;

        public CarAccessoryBuilder()
        {
            _carAccessory = new CarAccessory();
        }

        public CarAccessoryBuilder WithId(Guid ID)
        {
            _carAccessory.ID = ID;
            return this;    
        }

        public CarAccessory Build()
        {
            return _carAccessory;
        }
    }
}