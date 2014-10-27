using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class GradeAccesory : GradeEquipmentItem, IGradeAccessory
    {
        
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.EquipmentCompareAccessory Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public GradeAccesory(TMME.CarConfigurator.EquipmentCompareAccessory adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

    }
}