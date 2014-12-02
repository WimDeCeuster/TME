using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface ISubModel : IBaseObject
    {
        IPrice StartingPrice { get; }
        IReadOnlyList<IGrade> Grades { get; }

        IReadOnlyList<IAsset> Assets { get; }
        IReadOnlyList<ILink> Links { get; }
    }
}