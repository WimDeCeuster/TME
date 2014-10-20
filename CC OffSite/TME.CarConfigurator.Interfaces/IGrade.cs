using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Interfaces
{
    public interface IGrade : IBaseObject
    {
        bool Special { get; }
        IPrice StartingPrice { get; }
        IGrade BasedUpon { get; }

        IEnumerable<IVisibleInModeAndView> VisibleIn { get; }
        IEnumerable<IAsset> Assets { get; }
        
        IEnumerable<IGradeEquipmentItem> Equipment { get; }
        
    }
}
