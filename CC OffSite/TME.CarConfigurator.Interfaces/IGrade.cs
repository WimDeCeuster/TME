using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface IGrade : IBaseObject
    {
        String FeatureText { get; }
        String LongDescription { get; }
        Boolean Special { get; }

        IGrade BasedUpon { get; }
        IPrice StartingPrice { get; }

        IEnumerable<IAsset> Assets { get; }
    }
}
