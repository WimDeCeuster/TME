using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;

namespace TME.CarConfigurator.Interfaces
{
    public interface IGrade : IBaseObject
    {
        bool Special { get; }
        IPrice StartingPrice { get; }
        IGrade BasedUpon { get; }

        IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
        IReadOnlyList<IAsset> Assets { get; }
        
        IGradeEquipment Equipment { get; }
        IReadOnlyList<IGradePack> Packs { get; }
        
    }
}