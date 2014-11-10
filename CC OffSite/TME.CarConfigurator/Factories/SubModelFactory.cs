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
        private readonly IGradeFactory _gradeFactory;

        public SubModelFactory(ISubModelService subModelService,IAssetFactory assetFactory,IGradeFactory gradeFactory)
        {
            if (subModelService == null) throw new ArgumentNullException("subModelService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (gradeFactory == null) throw new ArgumentNullException("gradeFactory");

            _subModelService = subModelService;
            _assetFactory = assetFactory;
            _gradeFactory = gradeFactory;
        }

        public IReadOnlyList<ISubModel> GetSubModels(Publication publication, Context context)
        {
            return _subModelService.GetSubModels(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                .Select(subModel => new SubModel(subModel, publication, context, _assetFactory,_gradeFactory))
                .ToArray();
        }

        public ISubModel GetCarSubModel(Repository.Objects.SubModel subModel, Guid carID, Publication publication, Context context)
        {
            return new CarSubModel(subModel,publication,carID,context,_assetFactory,_gradeFactory);
        }
    }
}