using System;
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

        IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
        IReadOnlyList<IAsset> Assets { get; }
    }            
}
