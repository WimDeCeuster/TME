using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces
{
    public interface ITransmission : IBaseObject
    {
        ITransmissionType Type { get; }

        bool KeyFeature { get; }
        bool Brochure { get; }
        int NumberOfGears { get; }
   
        bool VisibleInExteriorSpin { get; }
        bool VisibleInInteriorSpin { get; }
        bool VisibleInXRay4X4Spin { get; }
        bool VisibleInXRayHybridSpin { get; }
        bool VisibleInXRaySafetySpin { get; }

        IEnumerable<IAsset> Assets { get; }
        
    }
}
