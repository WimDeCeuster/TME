using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Packs
{
    public interface IPack : IBaseObject
    {
        int ShortID { get; }
        bool GradeFeature { get; }
        bool OptionalGradeFeature { get; }

    }
}
