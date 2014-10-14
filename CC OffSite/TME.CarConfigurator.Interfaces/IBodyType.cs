using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface IBodyType : IBaseObject
    {
        int NumberOfDoors { get; }
        int NumberOfSeats { get; }

        bool VisibleInExteriorSpin { get; }
        bool VisibleInInteriorSpin { get; }
        bool VisibleInXRay4X4Spin { get; }
        bool VisibleInXRayHybridSpin { get; }
        bool VisibleInXRaySafetySpin { get; }

        IEnumerable<IAsset> Assets { get; }
        IEnumerable<IAsset> Get3DAssets(string view, string mode);
    }
}
