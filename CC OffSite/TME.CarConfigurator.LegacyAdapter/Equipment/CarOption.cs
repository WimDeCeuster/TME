using System;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class CarOption : CarEquipmentItem, ICarOption
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.CarOption Adaptee
        {
            get;
            set;
        }

        #endregion

        #region Constructor
        public CarOption(TMME.CarConfigurator.CarOption adaptee, TMME.CarConfigurator.Car carOfAdaptee)
            : base(adaptee, carOfAdaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public override IPrice TotalPrice
        {
            get { return new Price(Adaptee);}
        }

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
            get { throw new NotImplementedException(); }
        }

        public bool SuffixOption
        {
            get { throw new NotImplementedException(); }

        }
    }
}
