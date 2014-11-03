using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.TechnicalSpecifications
{
    public interface ICategory : IBaseObject
    {
        String Path { get; }
        ICategory Parent { get; }
        IReadOnlyList<ICategory> Categories { get; }
    }
}
