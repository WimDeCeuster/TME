using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class CarAccessory : CarEquipmentItem, ICarAccessory
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarAccessory Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CarAccessory(TMME.CarConfigurator.CarAccessory adaptee, TMME.CarConfigurator.Car carOfAdaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        public override IPrice TotalPrice
        {
            get { return new Price(Adaptee); }
        }

        public IPrice BasePrice
        {
            get { return new Price
                            {
                                PriceExVat = Adaptee.PriceExVat,
                                PriceInVat =  Adaptee.PriceInVat
                            }; 
            }
        }

        public IMountingCosts MountingCostsOnNewVehicle
        {
            get
            {
                return new MountingCosts(Adaptee.NewVehilceInstallationPriceExVat, Adaptee.NewVehilceInstallationPriceInVat, Adaptee.NewVehilceInstallationTime);
            }
        }

        public IMountingCosts MountingCostsOnUsedVehicle
        {
            get
            {
                return new MountingCosts(Adaptee.ExistingVehilceInstallationPriceExVat, Adaptee.ExistingVehilceInstallationPriceInVat, Adaptee.ExistingVehilceInstallationTime);
            }
        }
    }
}
