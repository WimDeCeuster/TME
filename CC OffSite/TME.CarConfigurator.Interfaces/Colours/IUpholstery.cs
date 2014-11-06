using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Colours
{
    public interface IUpholstery : IBaseObject
    {
        string InteriorColourCode { get; }
        string TrimCode { get; }
        IUpholsteryType Type { get; }

        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
        IReadOnlyList<IAsset> Assets { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
   }
}
