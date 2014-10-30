using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class Option : EquipmentItem, IOption
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarOption Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Option(TMME.CarConfigurator.CarOption adaptee, TMME.CarConfigurator.Generation generationOfAdaptee)
            : base(adaptee, generationOfAdaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public bool TechnologyItem
        {
            get { return Adaptee.TechnologyItem; }
        }

        public IOptionInfo ParentOption
        {
            get
            {
                var parentOption = Adaptee.ParentOption;
                if (parentOption == null) return null;
                return new OptionInfo(parentOption);
            }
        }
    }
}