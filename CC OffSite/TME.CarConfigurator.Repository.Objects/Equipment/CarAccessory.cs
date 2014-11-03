using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class CarAccessory :  CarEquipmentItem
    {

        public Price BasePrice { get; set; }
        public MountingCosts MountingCostsOnNewVehicle { get; set; }
        public MountingCosts MountingCostsOnUsedVehicle { get; set; }
    }
}