using System;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class MountingCosts : IMountingCosts
    {
        private readonly Repository.Objects.Equipment.MountingCosts _mountingCosts;

        public MountingCosts(Repository.Objects.Equipment.MountingCosts mountingCosts)
        {
            if (mountingCosts == null) throw new ArgumentNullException("mountingCosts");
            _mountingCosts = mountingCosts;
        }

        public IPrice Price { get { return new Price(_mountingCosts.Price); } }
        public TimeSpan Time { get { return _mountingCosts.Time;} }
    }
}