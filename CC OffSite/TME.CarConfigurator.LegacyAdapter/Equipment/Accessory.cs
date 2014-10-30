using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class Accessory : EquipmentItem, IAccessory
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarAccessory Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Accessory(TMME.CarConfigurator.CarAccessory adaptee, TMME.CarConfigurator.Generation generationOfAdaptee)
            : base(adaptee, generationOfAdaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

    }
}

