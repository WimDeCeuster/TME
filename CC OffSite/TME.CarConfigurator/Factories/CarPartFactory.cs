using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class CarPartFactory : ICarPartFactory
    {
        private ICarPartService _carPartService;

        public CarPartFactory(ICarPartService carPartService)
        {
            if (carPartService == null) throw new ArgumentNullException("carPartService");
            _carPartService = carPartService;
        }

        public IReadOnlyList<Repository.Objects.CarPart> GetCarCarParts(Guid carID, Publication publication, Context context)
        {
            return _carPartService.GetCarParts(publication.ID, carID, context).ToList();
        }
    }
}