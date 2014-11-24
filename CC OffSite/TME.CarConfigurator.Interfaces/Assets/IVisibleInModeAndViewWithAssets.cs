using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.Assets
{
    public interface IVisibleInModeAndViewWithAssets : IVisibleInModeAndView
    {
        IReadOnlyList<IAsset> Assets { get; }
    }
}