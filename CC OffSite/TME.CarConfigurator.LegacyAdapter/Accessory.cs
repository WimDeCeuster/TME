using TME.CarConfigurator.Interfaces.Equipment;
using Legacy = TMME.CarConfigurator;



namespace TME.CarConfigurator.LegacyAdapter
{
    public class Accessory : EquipmentItem, IAccessory
    {
        #region Dependencies (Adaptee)
        private Legacy.CarAccessory Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Accessory(Legacy.CarAccessory adaptee, Legacy.Generation generationOfAdaptee)
            : base(adaptee, generationOfAdaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

    }
}

