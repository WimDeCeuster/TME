using TME.CarConfigurator.Repository.Objects.Core;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Repository.Objects.Packs
{
    public class CarPackEquipmentItem : CarEquipmentItem
    {
        public Price Price { get; set; }
        public ColouringModes ColouringModes { get; set; }
    }
}
