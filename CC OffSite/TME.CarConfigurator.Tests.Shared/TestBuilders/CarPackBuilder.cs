using System;
using System.Linq;
using TME.CarConfigurator.Repository.Objects.Colours;
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

        public CarPackBuilder WithAvailableForExteriorColours(params ExteriorColourInfo[] exteriorColours)
        {
            _carPack.AvailableForExteriorColours = exteriorColours.ToList();
            return this;
        }

        public CarPackBuilder WithAvailableForUpholsteries(params UpholsteryInfo[] upholsteries)
        {
            _carPack.AvailableForUpholsteries = upholsteries.ToList();
            return this;
        }

        public CarPackBuilder WithCarPackEquipment(CarPackEquipment carPackEquipment)
        {
            _carPack.Equipment = carPackEquipment;
            return this;
        }

        public CarPack Build()
        {
            return _carPack;
        }
    }
}