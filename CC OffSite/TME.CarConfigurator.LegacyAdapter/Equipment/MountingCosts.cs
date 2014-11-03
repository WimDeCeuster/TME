using System;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class MountingCosts : IMountingCosts
    {


        public MountingCosts(decimal installationPriceExVat, decimal installationPriceInVat, TimeSpan installationTime)
        {
            Price = new Price()
            {
                PriceExVat = installationPriceExVat,
                PriceInVat = installationPriceInVat
            };
            Time = installationTime;
        }

        public IPrice Price
        {
            get;
            private set;
        }

        public TimeSpan Time
        {
            get;
            private set;
        }
    }
}