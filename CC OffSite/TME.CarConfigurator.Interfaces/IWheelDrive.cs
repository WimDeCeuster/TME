using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface IWheelDrive : IBaseObject
    {
        Boolean KeyFeature { get; }
        Boolean Brochure { get; }

        IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
        IReadOnlyList<IAsset> Assets { get; }

        [Obsolete]
        bool VisibleInExteriorSpin { get; }
        [Obsolete]
        bool VisibleInInteriorSpin { get; }
        [Obsolete]
        bool VisibleInXRay4X4Spin { get; }
        [Obsolete]
        bool VisibleInXRayHybridSpin { get; }
        [Obsolete]
        bool VisibleInXRaySafetySpin { get; }
    }
}
