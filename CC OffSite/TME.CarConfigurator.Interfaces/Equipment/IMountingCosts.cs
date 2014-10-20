using System;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Interfaces.Equipment
{
    public interface IMountingCosts
    {
        IPrice Price { get; }
        TimeSpan Time { get; }
    }
}
