using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Facades;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Factories
{
    public class ModelFactory : IModelFactory
    {
        private readonly IModelService _serviceFacade;
        private readonly IPublicationFactory _publicationFactory;

        public ModelFactory(IServiceFacade serviceFacade, IPublicationFactory publicationFactory)
        {
            if (serviceFacade == null) throw new ArgumentNullException("serviceFacade");
            if (publicationFactory == null) throw new ArgumentNullException("publicationFactory");

            _serviceFacade = serviceFacade.CreateModelService();
            _publicationFactory = publicationFactory;
        }

        public IEnumerable<IModel> GetModels(Context context)
        {
            var repositoryModels = _serviceFacade.GetModels(context).Where(HasActivePublicationsThatAreCurrentlyAvailable);

            var convertedModels = repositoryModels.Select(repositoryModel => CreateModel(repositoryModel, context));

            return convertedModels;
        }

        private static bool HasActivePublicationsThatAreCurrentlyAvailable(Repository.Objects.Model model)
        {
            return model.Publications.Any(p => p.State == PublicationState.Activated && p.LineOffFrom <= DateTime.Now && DateTime.Now <= p.LineOffTo);
        }

        private IModel CreateModel(Repository.Objects.Model repositoryModel, Context context)
        {
            return new Model(repositoryModel, context, _publicationFactory);
        }
    }
}