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
    public class CarFactory : ICarFactory
    {
        private readonly ICarService _carService;
        private readonly IBodyTypeFactory _bodyTypeFactory;

        public CarFactory(ICarService carService, IBodyTypeFactory bodyTypeFactory)
        {
            if (carService == null) throw new ArgumentNullException("carService");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");

            _carService = carService;
            _bodyTypeFactory = bodyTypeFactory;
        }

        public IReadOnlyList<ICar> GetCars(Publication publication, Context context)
        {
            return _carService.GetCars(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(car => new Car(car, publication, context, _bodyTypeFactory))
                                 .ToArray();
        }
    }
}
