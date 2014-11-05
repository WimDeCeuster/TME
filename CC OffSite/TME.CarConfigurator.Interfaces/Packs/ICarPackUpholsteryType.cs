using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface ICarPackUpholsteryType : ICarPackEquipmentItem
    {
        IReadOnlyList<IColourCombinationInfo> ColourCombinations { get; }
    }
}