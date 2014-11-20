using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class CarPackEquipmentBuilder
    {
        private CarPackEquipment _equipment;

        public CarPackEquipmentBuilder()
        {
            _equipment = new CarPackEquipment {
                Accessories = new List<CarPackAccessory>(),
                Options = new List<CarPackOption>(),
                ExteriorColourTypes = new List<CarPackExteriorColourType>(),
                UpholsteryTypes = new List<CarPackUpholsteryType>()
            };
        }

        public CarPackEquipmentBuilder WithOptions(params CarPackOption[] options)
        {
            _equipment.Options = options.ToList();
            return this;
        }

        public CarPackEquipmentBuilder WithExteriorColourTypes(params CarPackExteriorColourType[] types)
        {
            _equipment.ExteriorColourTypes = types.ToList();
            return this;
        }

        public CarPackEquipmentBuilder WithUpholsteryTypes(params CarPackUpholsteryType[] types)
        {
            _equipment.UpholsteryTypes = types.ToList();
            return this;
        }

        public CarPackEquipment Build()
        {
            return _equipment;
        }
    }
}
