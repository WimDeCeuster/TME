using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface ICarPackExteriorColourType : ICarPackEquipmentItem
    {
        IReadOnlyList<IColourCombinationInfo> ColourCombinations { get; }
    }
}