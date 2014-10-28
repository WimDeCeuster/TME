using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Colours
{
    public interface IUphostery : IBaseObject
    {
        string InteriorColourCode { get; }
        string TrimCode { get; }
        IUpholsteryType Type { get; }

        IEnumerable<IAsset> Assets { get; }
   }
}
