using System;
using System.Linq;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Factories
{
    public class ModelFactory : IModelFactory
    {
        private readonly IModelRepository _modelRepository;
        private readonly IPublicationFactory _publicationFactory;

        public ModelFactory(IModelRepository modelRepository, IPublicationFactory publicationFactory)
        {
            if (modelRepository == null) throw new ArgumentNullException("modelRepository");
            if (publicationFactory == null) throw new ArgumentNullException("publicationFactory");

            _modelRepository = modelRepository;
            _publicationFactory = publicationFactory;
        }

        public IModels Get(IContext context)
        {
            var repositoryModels = _modelRepository.Get(context).Where(HasActivePublicationsThatAreCurrentlyAvailable);

            var convertedModels = repositoryModels.Select(GetModel);

            return new Models(convertedModels); 
        }

        private static bool HasActivePublicationsThatAreCurrentlyAvailable(Repository.Objects.Model model)
        {
            return model.Publications.Any(p => p.State == PublicationState.Activated && p.LineOffFrom <= DateTime.Now && DateTime.Now <= p.LineOffTo);
        }

        private Model GetModel(Repository.Objects.Model repositoryModel)
        {
            return new Model(repositoryModel, _publicationFactory);
        }
    }
}