using TME.CarConfigurator.Interfaces.Enums;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface ICarPackEquipmentItem : ICarEquipmentItem
    {
        ColouringModes ColouringModes { get; }
    }
}
