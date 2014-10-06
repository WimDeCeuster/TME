using Spring.Context.Support;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository;

namespace TME.CarConfigurator
{
    public class Models : ReadOnlyList<IModel>, IModels
    {
        public static IModels GetModels(IModelRepository repository, IContext context)
        {
            // as Models is the entry point into the library, it can call upon the DI container. All objects in the sub hierarchy (cars, grades, ...) should have the repositories they need injected

            repository = repository ?? (IModelRepository)ContextRegistry.GetContext().GetObject("ModelRepository");

            return repository.GetModels(context);
        }
    }
}
