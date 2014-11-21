using TME.CarConfigurator.Interfaces.Packs;
using Legacy = TMME.CarConfigurator;


namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class CarPackAccessory : CarPackEquipmentItem, ICarPackAccessory
    {
        #region Dependencies (Adaptee)
        private Legacy.CarPackAccessory Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public CarPackAccessory(Legacy.CarPackAccessory adaptee, Legacy.CarAccessory standAloneItemOfTheAdaptee)
            : base(adaptee, standAloneItemOfTheAdaptee)
        {
            Adaptee = adaptee;
        }
        #endregion
    }
}
