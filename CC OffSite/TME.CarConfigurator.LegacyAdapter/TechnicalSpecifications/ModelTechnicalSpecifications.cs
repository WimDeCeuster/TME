using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.TechnicalSpecifications
{
    public class ModelTechnicalSpecifications : IModelTechnicalSpecifications
    {
        public IReadOnlyList<ICategory> Categories
        {
            get
            {
                var categories = TMME.CarConfigurator.TechnicalSpecifications.Categories.GetCategories(MyContext.CurrentContext);
                return categories.Cast<TMME.CarConfigurator.TechnicalSpecifications.Category>().Select(x => new Category(x)).ToList();
            }
        }
    }
}
