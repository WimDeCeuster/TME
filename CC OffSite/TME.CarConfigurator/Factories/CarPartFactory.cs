using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class CarPartFactory : ICarPartFactory
    {
        private readonly ICarPartService _carPartService;
        private readonly IAssetFactory _assetFactory;

        public CarPartFactory(ICarPartService carPartService,IAssetFactory assetFactory)
        {
            if (carPartService == null) throw new ArgumentNullException("carPartService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _carPartService = carPartService;
            _assetFactory = assetFactory;
        }

        public IReadOnlyList<ICarPart> GetCarParts(Guid carID, Publication publication, Context context)
        {
            return _carPartService.GetCarParts(publication.ID, carID, context).ToList().Select(carPart => new CarCarPart(carPart,publication,carID,context,_assetFactory)).ToList();
        }
    }
}