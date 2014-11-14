using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarEquipmentBuilder
    {
        private IReadOnlyList<CarAccessory> _accessories = A.Fake<IReadOnlyList<CarAccessory>>();
        private IReadOnlyList<CarOption> _options = A.Fake<IReadOnlyList<CarOption>>();

        public CarEquipmentBuilder WithAccessories(params CarAccessory[] carAccessories)
        {
            _accessories = carAccessories;
            return this;
        }

        public CarEquipmentBuilder WithOptions(params CarOption[] carOptions)
        {
            _options = carOptions;
            return this;
        }

        public CarEquipment Build()
        {
            return new CarEquipment(){Accessories = _accessories, Options = _options};
        }
    }
}