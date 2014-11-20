using System;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;

namespace TME.CarConfigurator.LegacyAdapter.TechnicalSpecifications
{
    public class  CategoryInfo : ICategoryInfo
    {
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.TechnicalSpecifications.Category Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public CategoryInfo(TMME.CarConfigurator.TechnicalSpecifications.Category adaptee)
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
