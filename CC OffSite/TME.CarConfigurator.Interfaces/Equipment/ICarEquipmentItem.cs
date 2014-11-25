
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Rules;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface ICarEquipmentItem : IEquipmentItem
    {
        bool Standard { get; }
        bool Optional { get; }

        IPrice Price { get; }
        
// ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
        IReadOnlyList<IAsset> Assets { get; }

        IReadOnlyList<IExteriorColourInfo> AvailableForExteriorColours { get; }
        IReadOnlyList<IUpholsteryInfo> AvailableForUpholsteries { get; }
        IRuleSets Rules { get; }
// ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}
