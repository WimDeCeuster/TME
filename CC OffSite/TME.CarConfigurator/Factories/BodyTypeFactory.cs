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
    public class BodyTypeFactory : IBodyTypeFactory
    {
        private readonly IBodyTypeService _bodyTypeService;
        private readonly IAssetFactory _assetFactory;

        public BodyTypeFactory(IBodyTypeService bodyTypeService, IAssetFactory assetFactory)
        {
            if (bodyTypeService == null) throw new ArgumentNullException("bodyTypeService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _bodyTypeService = bodyTypeService;
            _assetFactory = assetFactory;
        }

        public IEnumerable<IBodyType> GetBodyTypes(Publication publication, Context context)
        {
            var currentTimeFrame = publication.GetCurrentTimeFrame();

            var repoBodyTypes = _bodyTypeService.GetBodyTypes(publication.ID, currentTimeFrame.ID, context);

            return repoBodyTypes.Select(bt => new BodyType(bt, publication, context, _assetFactory)).ToArray();
        }

        public IBodyType GetCarBodyType(Repository.Objects.BodyType bodyType, Guid carId, Publication publication, Context context)
        {
            return new CarBodyType(bodyType, publication, carId, context, _assetFactory);
        }
    }
}