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
        private readonly IEngineFactory _engineFactory;

        public CarFactory(ICarService carService, IBodyTypeFactory bodyTypeFactory, IEngineFactory engineFactory)
        {
            if (carService == null) throw new ArgumentNullException("carService");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");
            if (engineFactory == null) throw new ArgumentNullException("engineFactory");

            _carService = carService;
            _bodyTypeFactory = bodyTypeFactory;
            _engineFactory = engineFactory;
        }

        public IReadOnlyList<ICar> GetCars(Publication publication, Context context)
        {
            return _carService.GetCars(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(car => new Car(car, publication, context, _bodyTypeFactory, _engineFactory))
                                 .ToArray();
        }
    }
}
