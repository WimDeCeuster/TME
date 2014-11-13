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
        private readonly ITransmissionFactory _transmissionFactory;
        private readonly IWheelDriveFactory _wheelDriveFactory;
        private readonly IGradeFactory _gradeFactory;
        private readonly ISubModelFactory _subModelFactory;
        private readonly ICarPartFactory _carPartFactory;

        public CarFactory(ICarService carService, IBodyTypeFactory bodyTypeFactory, IEngineFactory engineFactory, ITransmissionFactory transmissionFactory, IWheelDriveFactory wheelDriveFactory, IGradeFactory gradeFactory, ISubModelFactory subModelFactory, ICarPartFactory carPartFactory)
        {
            if (carService == null) throw new ArgumentNullException("carService");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");
            if (engineFactory == null) throw new ArgumentNullException("engineFactory");
            if (transmissionFactory == null) throw new ArgumentNullException("transmissionFactory");
            if (wheelDriveFactory == null) throw new ArgumentNullException("wheelDriveFactory");
            if (gradeFactory == null) throw new ArgumentNullException("gradeFactory");
            if (subModelFactory == null) throw new ArgumentNullException("subModelFactory");
            if (carPartFactory == null) throw new ArgumentNullException("carPartFactory");

            _carService = carService;
            _bodyTypeFactory = bodyTypeFactory;
            _engineFactory = engineFactory;
            _transmissionFactory = transmissionFactory;
            _wheelDriveFactory = wheelDriveFactory;
            _gradeFactory = gradeFactory;
            _subModelFactory = subModelFactory;
            _carPartFactory = carPartFactory;
        }

        public IReadOnlyList<ICar> GetCars(Publication publication, Context context)
        {
            return _carService.GetCars(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(car => new Car(car, publication, context, _bodyTypeFactory, _engineFactory, _transmissionFactory, _wheelDriveFactory, _gradeFactory, _subModelFactory,_carPartFactory))
                                 .ToArray();
        }
    }
}
