using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.LegacyAdapter.Equipment;
using Legacy = TMME.CarConfigurator;


namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class CarPackOption : CarPackEquipmentItem, ICarPackOption
    {
        #region Dependencies (Adaptee)
        private Legacy.CarPackOption Adaptee
        {
            get;
            set;
        }
        private Legacy.CarOption StandAloneItemOfTheAdaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public CarPackOption(Legacy.CarPackOption adaptee, Legacy.CarOption standAloneItemOfTheAdaptee)
            : base(adaptee, standAloneItemOfTheAdaptee)
        {
            Adaptee = adaptee;
            StandAloneItemOfTheAdaptee = standAloneItemOfTheAdaptee;
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
                return Adaptee.ParentOption == null
                    ? null
                    : new OptionInfo(Adaptee.ParentOption);
            }
        }

        public bool PostProductionOption
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool SuffixOption
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
