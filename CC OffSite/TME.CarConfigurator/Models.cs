using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;

namespace TME.CarConfigurator
{
    public class Models : List<IModel>, IModels
    {
        public Models(IEnumerable<Model> intermediateModels)
            : base(intermediateModels)
        {
        }

        public static IModels GetModels(IContext context, IModelRepository repository = null)
        {
            // as Models is the entry point into the library, it can call upon the DI container. All objects in the sub hierarchy (cars, grades, ...) should have the repositories they need injected
            repository = repository ?? null; // TODO: Get from DI container (poor man's DI)

            return repository.Get(context);
        }
    }
}
