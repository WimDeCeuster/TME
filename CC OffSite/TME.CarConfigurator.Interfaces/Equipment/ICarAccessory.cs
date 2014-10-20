using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface ICarAccessory : ICarEquipmentItem, IAccessory
    {
        IPrice BasePrice { get; }
        IMountingCosts MountingCostsOnNewVehicle { get; }
        IMountingCosts MountingCostsOnUsedVehicle { get; }
    }
}