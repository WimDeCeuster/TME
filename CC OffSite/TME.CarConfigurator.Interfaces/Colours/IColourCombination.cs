using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Interfaces.Colours
{
    public interface IColourCombination
    {
        Guid ID { get; }
        IExteriorColour ExteriorColour { get; }
        IUphostery Upholstery { get; }
        int SortIndex { get; }

        IEnumerable<IAsset> Assets { get; }
    }
}
