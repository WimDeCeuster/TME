using System;
using System.Linq;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

namespace TME.CarConfigurator.Factories
{
    public class ModelFactory : IModelFactory
    {
        private readonly IModelService _modelService;
        private readonly IPublicationFactory _publicationFactory;

        public ModelFactory(IModelService modelService, IPublicationFactory publicationFactory)
        {
            if (modelService == null) throw new ArgumentNullException("modelService");
            if (publicationFactory == null) throw new ArgumentNullException("publicationFactory");

            _modelService = modelService;
            _publicationFactory = publicationFactory;
        }

        public IModels GetModels(Context context)
        {
            var repositoryModels = _modelService.GetModels(context).Where(HasActivePublicationsThatAreCurrentlyAvailable);

            var convertedModels = repositoryModels.Select(repositoryModel => CreateModel(repositoryModel, context));

            return new Models(convertedModels); 
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