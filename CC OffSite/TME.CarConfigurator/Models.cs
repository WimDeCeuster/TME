using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.Repository.Objects.Enums;

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
            repository = repository ?? null; // TODO: Get from DI container (poor man's DI) instead of null

            var repositoryModels = repository.Get(context).Where(HasActivePublicationsThatAreCurrentlyAvailable);

            return new Models(repositoryModels.Select(m => new Model(m)));
        }

        private static bool HasActivePublicationsThatAreCurrentlyAvailable(Repository.Objects.Model model)
        {
            return model.Publications.Any(p => p.State == PublicationState.Activated && p.LineOffFrom <= DateTime.Now && DateTime.Now <= p.LineOffTo);
        }
    }
}
