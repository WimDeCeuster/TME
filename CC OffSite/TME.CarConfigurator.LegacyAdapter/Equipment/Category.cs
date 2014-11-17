using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class Category : BaseObject, ICategory
    {
        
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.Equipment.Category Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Category(TMME.CarConfigurator.Equipment.Category adaptee)
            : base(adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public string Path
        {
            get { return Adaptee.Path; }
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
                return  Adaptee.Categories.Cast<TMME.CarConfigurator.Equipment.Category>().Select(x => new Category(x)).ToList();
            }
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() * 29 + this.GetType().GetHashCode();
        }
    }
}
