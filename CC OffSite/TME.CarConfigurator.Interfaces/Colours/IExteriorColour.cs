using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Colours
{
    public interface IExteriorColour : IBaseObject
    {
        bool Promoted { get; }
        IColourTransformation Transformation { get; }
        IExteriorColourType Type { get; }

        IReadOnlyList<IAsset> Assets { get; }
    }
}
