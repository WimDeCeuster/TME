using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface IEngineCategory : IBaseObject
    {
        IEnumerable<IAsset> Assets { get; }
    }
}
