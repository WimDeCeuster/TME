using System;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarOptionBuilder
    {
        private readonly CarOption _carOption;

        public CarOptionBuilder()
        {
            _carOption = new CarOption();
        }

        public CarOptionBuilder WithId(Guid ID)
        {
            _carOption.ID = ID;
            return this;    
        }

        public CarOption Build()
        {
            return _carOption;
        }
    }
}