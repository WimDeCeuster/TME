using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Interfaces.Colours
{
    public interface IColourCombination
    {
        Guid ID { get; }
        IExteriorColour ExteriorColour { get; }
        IUpholstery Upholstery { get; }
        int SortIndex { get; }

        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
        IReadOnlyList<IAsset> Assets { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}
