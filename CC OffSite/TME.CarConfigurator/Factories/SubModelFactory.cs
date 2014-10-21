using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class SubModelFactory : ISubModelFactory
    {
        private readonly ISubModelService _subModelService;
        private readonly IAssetFactory _assetFactory;

        public SubModelFactory(ISubModelService subModelService,IAssetFactory assetFactory)
        {
            if (subModelService == null) throw new ArgumentNullException("subModelService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            _subModelService = subModelService;
            _assetFactory = assetFactory;
        }

        public IReadOnlyList<ISubModel> GetSubModels(Publication publication, Context context)
        {
            return _subModelService.GetSubModels(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                .Select(subModel => new SubModel(subModel, publication, context, _assetFactory))
                .ToArray();
        }
    }
}