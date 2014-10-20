using TME.CarConfigurator.Interfaces.Equipment;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class Option : EquipmentItem, IOption
    {
        #region Dependencies (Adaptee)
        private Legacy.CarOption Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Option(Legacy.CarOption adaptee, Legacy.Generation generationOfAdaptee)
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