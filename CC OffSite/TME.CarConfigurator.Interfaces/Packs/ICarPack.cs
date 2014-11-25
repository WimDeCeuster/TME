using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Rules;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface ICarPack : IPack
    {
        bool Standard { get; }
        bool Optional { get; }

        IPrice Price { get; }

        IReadOnlyList<IAsset> Assets { get; }
        ICarPackEquipment Equipment { get; }

        IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours { get; }
        IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries { get; }
        IReadOnlyList<IAccentColourCombination> AccentColourCombinations { get; }
        IRuleSets Rules { get; }
        
    }
}
