using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Assets;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;


namespace TME.CarConfigurator
{
    public class Model : Core.BaseObject<Repository.Objects.Model>, IModel
    {
        private readonly Repository.Objects.Context _repositoryContext;
        private readonly IPublicationFactory _publicationFactory;
        private readonly IBodyTypeFactory _bodyTypeFactory;
        private readonly IEngineFactory _engineFactory;
        private readonly ITransmissionFactory _transmissionFactory;
        private readonly IWheelDriveFactory _wheelDriveFactory;
        private readonly ISteeringFactory _steeringFactory;
        private readonly IGradeFactory _gradeFactory;
        private readonly ICarFactory _carFactory;
        private readonly ISubModelFactory _subModelFactory;
        private readonly IColourFactory _colourFactory;
        private readonly IEquipmentFactory _equipmentFactory;
        private readonly ISpecificationsFactory _specificationsFactory;

        private Repository.Objects.Publication _repositoryPublication;
        private IReadOnlyList<IAsset> _assets;
        private IReadOnlyList<ILink> _links;
        private IReadOnlyList<IBodyType> _bodyTypes;
        private IReadOnlyList<IEngine> _engines;
        private IReadOnlyList<ITransmission> _transmissions;
        private IReadOnlyList<IWheelDrive> _wheelDrives;
        private IReadOnlyList<ISteering> _steerings;
        private IReadOnlyList<IGrade> _grades;
        private IReadOnlyList<ICar> _cars;
        private IReadOnlyList<ISubModel> _subModels;
        private IReadOnlyList<IColourCombination> _colourCombinations;
        private IModelEquipment _equipment;
        private IModelTechnicalSpecifications _specifications;

        private CarConfiguratorVersion _carConfiguratorVersion;

        private Repository.Objects.Publication RepositoryPublication
        {
            get
            {
                _repositoryPublication = _repositoryPublication ?? _publicationFactory.GetPublication(RepositoryObject, _repositoryContext);
                return _repositoryPublication;
            }
        }

        public string Brand { get { return RepositoryObject.Brand; } }

        public bool Promoted { get { return RepositoryObject.Promoted; } }

        public string SSN { get { return RepositoryPublication.Generation.SSN; } }

        public ICarConfiguratorVersion CarConfiguratorVersion { get { return _carConfiguratorVersion = _carConfiguratorVersion ?? new CarConfiguratorVersion(RepositoryPublication.Generation.CarConfiguratorVersion); } }

        public IReadOnlyList<ILink> Links { get { return _links = _links ?? RepositoryPublication.Generation.Links.Select(l => new Link(l)).ToArray(); } }

        public IReadOnlyList<IAsset> Assets { get { return _assets = _assets ?? RepositoryPublication.Generation.Assets.Select(a => new Asset(a)).ToArray(); } }

        public IReadOnlyList<IBodyType> BodyTypes { get { return _bodyTypes = _bodyTypes ?? _bodyTypeFactory.GetBodyTypes(RepositoryPublication, _repositoryContext); } }

        public IReadOnlyList<IEngine> Engines { get { return _engines = _engines ?? _engineFactory.GetEngines(RepositoryPublication, _repositoryContext); } }

        public IReadOnlyList<ITransmission> Transmissions { get { return _transmissions = _transmissions ?? _transmissionFactory.GetTransmissions(RepositoryPublication, _repositoryContext); } }

        public IReadOnlyList<IWheelDrive> WheelDrives { get { return _wheelDrives = _wheelDrives ?? _wheelDriveFactory.GetWheelDrives(RepositoryPublication, _repositoryContext); } }

        public IReadOnlyList<ISteering> Steerings { get { return _steerings = _steerings ?? _steeringFactory.GetSteerings(RepositoryPublication, _repositoryContext); } }

        public IReadOnlyList<IGrade> Grades { get { return _grades = _grades ?? _gradeFactory.GetGrades(RepositoryPublication, _repositoryContext); } }

        public IReadOnlyList<IFuelType> FuelTypes { get { throw new NotImplementedException(); } }

        public IReadOnlyList<ICar> Cars { get { return _cars = _cars ?? _carFactory.GetCars(RepositoryPublication, _repositoryContext); } }

        public IReadOnlyList<ISubModel> SubModels { get { return _subModels = _subModels ?? _subModelFactory.GetSubModels(RepositoryPublication, _repositoryContext); } }

        public IReadOnlyList<IColourCombination> ColourCombinations { get { return _colourCombinations = _colourCombinations ?? _colourFactory.GetColourCombinations(RepositoryPublication, _repositoryContext); } }

        public IModelEquipment Equipment { get { return _equipment = _equipment ?? _equipmentFactory.GetModelEquipment(RepositoryPublication, _repositoryContext); } }

        public IModelTechnicalSpecifications TechnicalSpecifications { get { return _specifications = _specifications ?? _specificationsFactory.GetModelSpecifications(RepositoryPublication, _repositoryContext); } }

        public Model(
            Repository.Objects.Model repositoryModel,
            Repository.Objects.Context repositoryContext,
            IPublicationFactory publicationFactory,
            IBodyTypeFactory bodyTypeFactory,
            IEngineFactory engineFactory,
            ITransmissionFactory transmissionFactory,
            IWheelDriveFactory wheelDriveFactory,
            ISteeringFactory steeringFactory,
            IGradeFactory gradeFactory,
            ICarFactory carFactory,
            ISubModelFactory subModelFactory,
            IColourFactory colourFactory,
            IEquipmentFactory equipmentFactory,
            ISpecificationsFactory specificationsFactory)
            : base(repositoryModel)
        {
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (publicationFactory == null) throw new ArgumentNullException("publicationFactory");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");
            if (engineFactory == null) throw new ArgumentNullException("engineFactory");
            if (transmissionFactory == null) throw new ArgumentNullException("transmissionFactory");
            if (wheelDriveFactory == null) throw new ArgumentNullException("wheelDriveFactory");
            if (steeringFactory == null) throw new ArgumentNullException("steeringFactory");
            if (gradeFactory == null) throw new ArgumentNullException("gradeFactory");
            if (carFactory == null) throw new ArgumentNullException("carFactory");
            if (subModelFactory == null) throw new ArgumentNullException("subModelFactory");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");
            if (equipmentFactory == null) throw new ArgumentNullException("equipmentFactory");
            if (specificationsFactory == null) throw new ArgumentNullException("specificationsFactory");

            _repositoryContext = repositoryContext;
            _publicationFactory = publicationFactory;
            _bodyTypeFactory = bodyTypeFactory;
            _engineFactory = engineFactory;
            _transmissionFactory = transmissionFactory;
            _wheelDriveFactory = wheelDriveFactory;
            _steeringFactory = steeringFactory;
            _gradeFactory = gradeFactory;
            _carFactory = carFactory;
            _subModelFactory = subModelFactory;
            _colourFactory = colourFactory;
            _equipmentFactory = equipmentFactory;
            _specificationsFactory = specificationsFactory;
        }
    }
}
