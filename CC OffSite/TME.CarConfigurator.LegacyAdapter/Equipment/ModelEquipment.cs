using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Equipment;
using TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class ModelEquipment : IModelEquipment
    {
        public IReadOnlyList<ICategory> Categories
        {
            get
            {
                var categories = TMME.CarConfigurator.Equipment.Categories.GetCategories(MyContext.CurrentContext);
                return categories.Cast<TMME.CarConfigurator.Equipment.Category>().Select(x => new Category(x)).ToList();
            }
        }
    }
}
