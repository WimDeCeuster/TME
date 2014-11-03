using System;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Equipment
{
    public class MountingCosts
    {
        public Price Price { get; set; }
        public TimeSpan Time { get; set; }
    }
}