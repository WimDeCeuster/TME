using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Interfaces.Colours
{
    public interface ICarColourCombination 
    {
        Guid ID { get; }
        ICarExteriorColour ExteriorColour { get; }
        ICarUpholstery Upholstery { get; }
        int SortIndex { get; }

        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
        IReadOnlyList<IAsset> Assets { get; }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}
