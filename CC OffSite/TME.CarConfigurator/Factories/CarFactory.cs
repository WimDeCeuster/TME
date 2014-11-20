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
        private readonly IEquipmentFactory _carEquipmentFactory;
        private readonly IPackFactory _packFactory;
        private readonly ISteeringFactory _steeringFactory;
        private readonly ISpecificationsFactory _specificationsFactory;

        public CarFactory(ICarService carService, IBodyTypeFactory bodyTypeFactory, IEngineFactory engineFactory, ITransmissionFactory transmissionFactory, IWheelDriveFactory wheelDriveFactory, IGradeFactory gradeFactory, ISubModelFactory subModelFactory, ICarPartFactory carPartFactory, IEquipmentFactory carEquipmentFactory, IPackFactory packFactory, ISteeringFactory steeringFactory, ISpecificationsFactory specificationsFactory)
        {
            if (carService == null) throw new ArgumentNullException("carService");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");
            if (engineFactory == null) throw new ArgumentNullException("engineFactory");
            if (transmissionFactory == null) throw new ArgumentNullException("transmissionFactory");
            if (wheelDriveFactory == null) throw new ArgumentNullException("wheelDriveFactory");
            if (gradeFactory == null) throw new ArgumentNullException("gradeFactory");
            if (subModelFactory == null) throw new ArgumentNullException("subModelFactory");
            if (carPartFactory == null) throw new ArgumentNullException("carPartFactory");
            if (carEquipmentFactory == null) throw new ArgumentNullException("carEquipmentFactory");
            if (packFactory == null) throw new ArgumentNullException("packFactory");
            if (steeringFactory == null) throw new ArgumentNullException("steeringFactory");
            if (specificationsFactory == null) throw new ArgumentNullException("specificationsFactory");

            _carService = carService;
            _bodyTypeFactory = bodyTypeFactory;
            _engineFactory = engineFactory;
            _transmissionFactory = transmissionFactory;
            _wheelDriveFactory = wheelDriveFactory;
            _gradeFactory = gradeFactory;
            _subModelFactory = subModelFactory;
            _carPartFactory = carPartFactory;
            _carEquipmentFactory = carEquipmentFactory;
            _packFactory = packFactory;
            _steeringFactory = steeringFactory;
            _specificationsFactory = specificationsFactory;
        }

        public IReadOnlyList<ICar> GetCars(Publication publication, Context context)
        {
            return _carService.GetCars(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(car => new Car(car, publication, context, _bodyTypeFactory, _engineFactory, _transmissionFactory, _wheelDriveFactory, _gradeFactory, _subModelFactory, _carPartFactory, _carEquipmentFactory, _packFactory, _steeringFactory, _specificationsFactory))
                                 .ToArray();
        }
    }
}
