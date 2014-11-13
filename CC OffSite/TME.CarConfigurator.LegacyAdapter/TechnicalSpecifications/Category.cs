using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;

namespace TME.CarConfigurator.LegacyAdapter.TechnicalSpecifications
{
    public class Category : BaseObject, ICategory
    {
        
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.TechnicalSpecifications.Category Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Category(TMME.CarConfigurator.TechnicalSpecifications.Category adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public string Path
        {
            get
            {
                var category = Adaptee;
                var path = category.Name;

                while (category.Parent !=null && category.Parent.ID != Guid.Empty)
                {
                    category = category.Parent;
                    path = string.Format("{0}/{1}", category.Name, path);
                }
                return path;
            }
        }


        public ICategory Parent
        {
            get
            {
                return Adaptee.Parent == null ? null : new Category(Adaptee.Parent);
            }
        }

        public IReadOnlyList<ICategory> Categories
        {
            get
            {
                return Adaptee.Categories.Cast<TMME.CarConfigurator.TechnicalSpecifications.Category>().Select(x => new Category(x)).ToList();
            }
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() * 29 + this.GetType().GetHashCode();
        }

    }
}
