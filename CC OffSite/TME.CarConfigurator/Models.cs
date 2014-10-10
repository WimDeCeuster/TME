using System.Collections.Generic;
using TME.CarConfigurator.Facades;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Facades;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class Models : List<IModel>
    {
        public static IEnumerable<IModel> GetModels(Context context)
        {
            var modelFactoryFacade = new ModelFactoryFacade();

            return GetModels(context, modelFactoryFacade);
        }

        public static IEnumerable<IModel> GetModels(Context context, IModelFactoryFacade modelFactoryFacade)
        {
            var modelFactory = modelFactoryFacade.Create();

            return modelFactory.GetModels(context);
        }
    }
}
