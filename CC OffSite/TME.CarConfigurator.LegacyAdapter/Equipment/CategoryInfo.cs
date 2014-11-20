using System;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class  CategoryInfo : ICategoryInfo
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.Equipment.Category Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CategoryInfo(TMME.CarConfigurator.Equipment.Category adaptee)
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
            get
            {
                var category = Adaptee;
                var path = category.Name;

                while (category.Parent != null && category.Parent.ID != Guid.Empty)
                {
                    category = category.Parent;
                    path = string.Format("{0}/{1}", category.Name, path);
                }
                return path;
            }
        }

        public int SortIndex
        {
            get { return Adaptee.SortIndex; }
        }
    }
}
