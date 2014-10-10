using System.Collections.Generic;
using TME.CarConfigurator.Facades;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Facades;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class Models : List<IModel>, IModels
    {
        public Models(IEnumerable<IModel> intermediateModels)
            : base(intermediateModels)
        {
        }

        public static IModels GetModels(Context context)
        {
            var modelFactoryFacade = new ModelFactoryFacade();

            return GetModels(context, modelFactoryFacade);
        }

        public static IModels GetModels(Context context, IModelFactoryFacade modelFactoryFacade)
        {
            var modelFactory = modelFactoryFacade.Create();

            return modelFactory.GetModels(context);
        }
    }
}
