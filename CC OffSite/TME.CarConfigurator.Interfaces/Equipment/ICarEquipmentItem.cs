
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface ICarEquipmentItem : IEquipmentItem
    {
        bool Standard { get; }
        bool Optional { get; }

        IPrice TotalPrice { get; }
    }
}
