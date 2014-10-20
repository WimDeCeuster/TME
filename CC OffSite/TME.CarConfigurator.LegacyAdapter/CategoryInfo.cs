using System;
using TME.CarConfigurator.Interfaces.Equipment;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class  CategoryInfo : ICategoryInfo
    {
        #region Dependencies (Adaptee)
        private Legacy.Equipment.Category Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CategoryInfo(Legacy.Equipment.Category adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public Guid ID
        {
            get { return Adaptee.ID; }
        }

        public string Path
        {
            get { return Adaptee.Path; }
        }

        public int SortIndex
        {
            get { return Adaptee.SortIndex; }
        }
    }
}
