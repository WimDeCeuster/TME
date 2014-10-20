using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class GradeOption : GradeEquipmentItem, IGradeOption
    {

        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.EquipmentCompareOption Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public GradeOption(TMME.CarConfigurator.EquipmentCompareOption adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public bool TechnologyItem
        {
            get { return ((TMME.CarConfigurator.CarOption) GetCarEquipmentItem()).TechnologyItem; }
        }

        public IOptionInfo ParentOption
        {
            get
            {
                var parentOption = ((TMME.CarConfigurator.CarOption) GetCarEquipmentItem()).ParentOption;
                if (parentOption == null) return null;
                return new OptionInfo(parentOption);
            }
        }
    }
}