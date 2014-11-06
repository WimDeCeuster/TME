using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface ICarPack : IPack
    {
        bool Standard { get; }
        bool Optional { get; }

        IPrice Price { get; }

        IReadOnlyList<IAsset> Assets { get; }
        
        IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours { get; }
        IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries { get; }
        ICarPackEquipment Equipment { get; }
    }
}
