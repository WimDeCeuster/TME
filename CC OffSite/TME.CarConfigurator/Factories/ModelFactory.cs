using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Factories
{
    public class ModelFactory : IModelFactory
    {
        private readonly IModelService _modelService;
        private readonly IPublicationFactory _publicationFactory;
        private readonly IAssetFactory _assetFactory;
        private readonly ILinkFactory _linkFactory;

        public ModelFactory(IModelService modelService, IPublicationFactory publicationFactory, IAssetFactory assetFactory, ILinkFactory linkFactory)
        {
            if (modelService == null) throw new ArgumentNullException("modelService");
            if (publicationFactory == null) throw new ArgumentNullException("publicationFactory");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (linkFactory == null) throw new ArgumentNullException("linkFactory");

            _modelService = modelService;
            _publicationFactory = publicationFactory;
            _assetFactory = assetFactory;
            _linkFactory = linkFactory;
        }

        public IEnumerable<IModel> GetModels(Context context)
        {
            var repositoryModels = _modelService.GetModels(context).Where(HasActivePublicationsThatAreCurrentlyAvailable);

            var convertedModels = repositoryModels.Select(repositoryModel => CreateModel(repositoryModel, context));

            return convertedModels;
        }

        private static bool HasActivePublicationsThatAreCurrentlyAvailable(Repository.Objects.Model model)
        {
            return model.Publications.Any(p => p.State == PublicationState.Activated && p.LineOffFrom <= DateTime.Now && DateTime.Now <= p.LineOffTo);
        }

        private IModel CreateModel(Repository.Objects.Model repositoryModel, Context context)
        {
            return new Model(repositoryModel, context, _publicationFactory, _assetFactory, _linkFactory);
        }
    }
}