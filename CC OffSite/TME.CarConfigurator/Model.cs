using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;


namespace TME.CarConfigurator
{
    public class Model : Core.BaseObject, IModel
    {
        private readonly Repository.Objects.Model _repositoryModel;
        private readonly Repository.Objects.Context _repositoryContext;
        private readonly IPublicationFactory _publicationFactory;
        private readonly IBodyTypeFactory _bodyTypeFactory;
        private readonly IEngineFactory _engineFactory;
        private readonly ITransmissionFactory _transmissionFactory;
        private readonly IWheelDriveFactory _wheelDriveFactory;
        private readonly ICarFactory _carFactory;

        private Repository.Objects.Publication _repositoryPublication;        
        private IEnumerable<IAsset> _assets;
        private IEnumerable<ILink> _links;
        private IEnumerable<IBodyType> _bodyTypes;
        private IEnumerable<IEngine> _engines;
        private IEnumerable<ITransmission> _transmissions;
        private IEnumerable<IWheelDrive> _wheelDrives;
        private IEnumerable<ICar> _cars;

        private CarConfiguratorVersion _carConfiguratorVersion;

        private Repository.Objects.Publication RepositoryPublication
        {
            get
            {
                _repositoryPublication = _repositoryPublication ?? _publicationFactory.GetPublication(_repositoryModel, _repositoryContext);
                return _repositoryPublication;
            }
        }

        public string Brand { get { return _repositoryModel.Brand; } }

        public bool Promoted { get { return _repositoryModel.Promoted; } }

        public string SSN { get { return RepositoryPublication.Generation.SSN; } }

        public ICarConfiguratorVersion CarConfiguratorVersion { get { return _carConfiguratorVersion = _carConfiguratorVersion ?? new CarConfiguratorVersion(RepositoryPublication.Generation.CarConfiguratorVersion); } }

        public IEnumerable<ILink> Links { get { return _links = _links ?? RepositoryPublication.Generation.Links.Select(l => new Link(l)).ToArray(); } }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? RepositoryPublication.Generation.Assets.Select(a => new Asset(a)).ToArray(); } }

        public IEnumerable<IBodyType> BodyTypes { get { return _bodyTypes = _bodyTypes ?? _bodyTypeFactory.GetBodyTypes(RepositoryPublication, _repositoryContext); } }

        public IEnumerable<IEngine> Engines { get { return _engines = _engines ?? _engineFactory.GetEngines(RepositoryPublication, _repositoryContext); } }

        public IEnumerable<ITransmission> Transmissions { get { return _transmissions = _transmissions ?? _transmissionFactory.GetTransmissions(RepositoryPublication, _repositoryContext); } }

        public IEnumerable<IWheelDrive> WheelDrives { get { return _wheelDrives = _wheelDrives ?? _wheelDriveFactory.GetWheelDrives(RepositoryPublication, _repositoryContext); } }

        public IEnumerable<IFuelType> FuelTypes { get { throw new NotImplementedException(); } }

        public IEnumerable<ICar> Cars { get { return _cars = _cars ?? _carFactory.GetCars(RepositoryPublication, _repositoryContext); } }

        public Model(
            Repository.Objects.Model repositoryModel,
            Repository.Objects.Context repositoryContext,
            IPublicationFactory publicationFactory,
            IBodyTypeFactory bodyTypeFactory,
            IEngineFactory engineFactory,
            ITransmissionFactory transmissionFactory,
            IWheelDriveFactory wheelDriveFactory,
            ICarFactory carFactory)
            : base(repositoryModel)
        {
            if (repositoryModel == null) throw new ArgumentNullException("repositoryModel");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (publicationFactory == null) throw new ArgumentNullException("publicationFactory");
            if (bodyTypeFactory == null) throw new ArgumentNullException("bodyTypeFactory");
            if (engineFactory == null) throw new ArgumentNullException("engineFactory");
            if (transmissionFactory == null) throw new ArgumentNullException("transmissionFactory");
            if (wheelDriveFactory == null) throw new ArgumentNullException("wheelDriveFactory");
            if (carFactory == null) throw new ArgumentNullException("carFactory");

            _repositoryModel = repositoryModel;
            _repositoryContext = repositoryContext;
            _publicationFactory = publicationFactory;
            _bodyTypeFactory = bodyTypeFactory;
            _engineFactory = engineFactory;
            _transmissionFactory = transmissionFactory;
            _wheelDriveFactory = wheelDriveFactory;
            _carFactory = carFactory;
        }
    }
}
