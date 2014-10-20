﻿using TME.CarConfigurator.Interfaces.Equipment;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class GradeOption : GradeEquipmentItem, IGradeOption
    {

        #region Dependencies (Adaptee)
        private Legacy.EquipmentCompareOption Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public GradeOption(Legacy.EquipmentCompareOption adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public bool TechnologyItem
        {
            get { return ((Legacy.CarOption)GetCarEquipmentItem()).TechnologyItem; }
        }

        public IOptionInfo ParentOption
        {
            get
            {
                var parentOption = ((Legacy.CarOption)GetCarEquipmentItem()).ParentOption;
                if (parentOption == null) return null;
                return new OptionInfo(parentOption);
            }
        }
    }
}