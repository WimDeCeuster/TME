using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class Car : BaseObject<Repository.Objects.Car>, ICar
    {
        readonly Publication _repositoryPublication;
        readonly Context _repositoryContext;
        readonly IBodyTypeFactory _bodyTypeFactory;
        readonly ITransmissionFactory _transmissionFactory;
        readonly IWheelDriveFactory _wheelDriveFactory;
        readonly IGradeFactory _gradeFactory;
        readonly ISubModelFactory _subModelFactory;
        readonly IEngineFactory _engineFactory;
        readonly ICarPartFactory _carPartFactory;
        readonly IEquipmentFactory _equipmentFactory;
        readonly IPackFactory _packFactory;
        readonly ISteeringFactory _steeringFactory;
        readonly ISpecificationsFactory _specificationsFactory;
        readonly IColourFactory _colourFactory;

        IPrice _basePrice;
        IPrice _startingPrice;
        IBodyType _bodyType;
        IEngine _engine;
        ITransmission _transmission;
        IWheelDrive _wheelDrive;
        IGrade _grade;
        ISubModel _subModel;
        ISteering _steering;
        IReadOnlyList<ICarPart> _carParts;
        ICarEquipment _carEquipment;
        IReadOnlyList<ICarPack> _carPacks;
        IReadOnlyList<ICarTechnicalSpecification> _carTechnicalSpecifications;
        IReadOnlyList<IColourCombination> _colourCombinations;

        public Car(Repository.Objects.Car repositoryCar,
        	Publication repositoryPublication, 
        	Context repositoryContext, 
        	IBodyTypeFactory bodyTypeFactory, 
        	IEngineFactory engineFactory, 
        	ITransmissionFactory transmissionFactory, 
        	IWheelDriveFactory wheelDriveFactory, 
        	IGradeFactory gradeFactory,       
        	ISubModelFactory subModelFactory, 
            ICarPartFactory carPartFactory,
            IEquipmentFactory equipmentFactory,
            IPackFactory packFactory,
            ISteeringFactory steeringFactory,
            ISpecificationsFactory specificationsFactory,
            IColourFactory colourFactory)
            : base(repositoryCar)
        {
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");
            if (engineFactory == null) throw new ArgumentNullException("engineFactory");
            if (transmissionFactory == null) throw new ArgumentNullException("transmissionFactory");
            if (wheelDriveFactory == null) throw new ArgumentNullException("wheelDriveFactory");
            if (gradeFactory == null) throw new ArgumentNullException("gradeFactory");
            if (subModelFactory == null) throw new ArgumentNullException("subModelFactory");
            if (carPartFactory == null) throw new ArgumentNullException("carPartFactory");
            if (equipmentFactory == null) throw new ArgumentNullException("equipmentFactory");
            if (packFactory == null) throw new ArgumentNullException("packFactory");
            if (steeringFactory == null) throw new ArgumentNullException("steeringFactory");
            if (specificationsFactory == null) throw new ArgumentNullException("specificationsFactory");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");

            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _bodyTypeFactory = bodyTypeFactory;
            _engineFactory = engineFactory;
            _transmissionFactory = transmissionFactory;
            _wheelDriveFactory = wheelDriveFactory;
            _gradeFactory = gradeFactory;
            _subModelFactory = subModelFactory;
            _carPartFactory = carPartFactory;
            _equipmentFactory = equipmentFactory;
            _packFactory = packFactory;
            _steeringFactory = steeringFactory;
            _specificationsFactory = specificationsFactory;
            _colourFactory = colourFactory;
        }

        public int ShortID { get { return RepositoryObject.ShortID; } }
        public bool Promoted { get { return RepositoryObject.Promoted; } }
        public bool WebVisible { get { return RepositoryObject.WebVisible; } }
        public bool ConfigVisible { get { return RepositoryObject.ConfigVisible; } }
        public bool FinanceVisible { get { return RepositoryObject.FinanceVisible; } }
        public IPrice BasePrice { get { return _basePrice = _basePrice  ?? new Price(RepositoryObject.BasePrice); } }
        public IPrice StartingPrice { get { return _startingPrice = _startingPrice ?? new Price(RepositoryObject.StartingPrice); } }
        public IBodyType BodyType { get { return _bodyType = _bodyType ?? _bodyTypeFactory.GetCarBodyType(RepositoryObject.BodyType, RepositoryObject.ID, _repositoryPublication, _repositoryContext); } }
        public IEngine Engine { get { return _engine = _engine ?? _engineFactory.GetCarEngine(RepositoryObject.Engine, RepositoryObject.ID, _repositoryPublication, _repositoryContext); } }
        public ITransmission Transmission { get { return _transmission = _transmission ?? _transmissionFactory.GetCarTransmission(RepositoryObject.Transmission, RepositoryObject.ID, _repositoryPublication, _repositoryContext);} }
        public IWheelDrive WheelDrive { get { return _wheelDrive = _wheelDrive ?? _wheelDriveFactory.GetCarWheelDrive(RepositoryObject.WheelDrive, RepositoryObject.ID, _repositoryPublication, _repositoryContext); } }
        public ISteering Steering { get { return _steering = _steering ?? _steeringFactory.GetCarSteering(RepositoryObject.Steering, RepositoryObject.ID, _repositoryPublication, _repositoryContext); } }
        public IGrade Grade { get { return _grade = _grade ??  _gradeFactory.GetCarGrade(RepositoryObject.Grade, RepositoryObject.ID, _repositoryPublication, _repositoryContext); } }
        public ISubModel SubModel { get { return _subModel = _subModel ?? _subModelFactory.GetCarSubModel(RepositoryObject.SubModel, RepositoryObject.ID, _repositoryPublication, _repositoryContext); } }
        
        public IReadOnlyList<ICarPart> Parts
        {
            get
            {
                return
                    _carParts =
                        _carParts ??
                        _carPartFactory.GetCarParts(RepositoryObject.ID, _repositoryPublication, _repositoryContext);
            }
        }

        public ICarEquipment Equipment
        {
            get
            {
                return
                    _carEquipment =
                        _carEquipment ??
                        _equipmentFactory.GetCarEquipment(RepositoryObject.ID, _repositoryPublication,
                            _repositoryContext);
            }
        }

        public IReadOnlyList<ICarPack> Packs
        {
            get { return _carPacks = _carPacks ?? _packFactory.GetCarPacks(_repositoryPublication, _repositoryContext, ID); }
        }

        public IReadOnlyList<ICarTechnicalSpecification> TechnicalSpecifications
        {
            get
            {
                return
                    _carTechnicalSpecifications =
                        _carTechnicalSpecifications ??
                        _specificationsFactory.GetCarTechnicalSpecifications(ID, _repositoryPublication,
                            _repositoryContext);
            }
        }

        public IReadOnlyList<ICarColourCombination> ColourCombinations
        {
            get { return _colourCombinations = _colourCombinations ?? _colourFactory.GetCarColourCombinations(_repositoryPublication, _repositoryContext, ID); } }
        }
    }
}