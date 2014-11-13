using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Interfaces
{
    public interface ICarPart
    {
         string Code { get; }
         string Name { get; }
         Guid ID { get; }

// ReSharper disable once ReturnTypeCanBeEnumerable.Global
         IReadOnlyList<IVisibleInModeAndView> VisibleIn { get; }
    }
}
