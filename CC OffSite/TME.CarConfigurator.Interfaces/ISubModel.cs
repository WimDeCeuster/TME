using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface ISubModel : IBaseObject
    {
        IGeneration Generation { get; }

        IEnumerable<IVisibleInModeAndView> VisibleIn { get; }
        IEnumerable<IAsset> Assets { get; } 
    }
}